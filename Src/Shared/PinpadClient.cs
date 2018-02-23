using Ladon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// The main class used for communicating with a Verifone Pin Pad via the POS Link protocol.
	/// </summary>
	/// <remarks>
	/// <para>Send requests (and receive responses) voa the <see cref="ProcessRequest{TRequest, TResponse}(TRequest)"/> method.</para>
	/// </remarks>
	public class PinpadClient
	{
		private readonly string _Address;
		private readonly int _Port;

		private readonly MessageWriter _Writer;
		private readonly MessageReader _Reader;

		/// <summary>
		/// Raised when there is an information prompt or status change that should be displayed to the user.
		/// </summary>
		/// <remarks>
		/// <para>This event may be raised from background threads, any code updating UI may need to invoke to the UI thread.</para>
		/// </remarks>
		public event EventHandler<DisplayMessageEventArgs> DisplayMessage;
		/// <summary>
		/// Raised when there is a question that must be answered by the operator.
		/// </summary>
		/// <remarks>
		/// <para>This event may be raised from background threads, any code updating UI may need to invoke to the UI thread.</para>
		/// </remarks>
		public event EventHandler<QueryOperatorEventArgs> QueryOperator;

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <remarks>
		/// <para>Uses the default port of 4444.</para>
		/// </remarks>
		/// <param name="address">The address or host name of the pin pad to connect to.</param>
		public PinpadClient(string address) : this(address, ProtocolConstants.DefaultPort)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="address">The address or host name of the pin pad to connect to.</param>
		/// <param name="port">The TCP/IP port of the pinpad to connect to.</param>
		public PinpadClient(string address, int port)
		{
			_Address = address.GuardNullOrWhiteSpace(nameof(address));
			_Port = port.GuardRange(nameof(port), 1, Int16.MaxValue);

			_Writer = new MessageWriter();
			_Reader = new MessageReader();
		}

		/// <summary>
		/// Sends a request to the pind pad and returns the response.
		/// </summary>
		/// <typeparam name="TRequestMessage">The type of request to send.</typeparam>
		/// <typeparam name="TResponseMessage">The type of response expected.</typeparam>
		/// <param name="requestMessage">The request message to be sent.</param>
		/// <returns>A instance of {TResponseMessage} containing the pin pad response.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestMessage"/> message is null, or if any required property of the request is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if any property of <paramref name="requestMessage"/> is invalid.</exception>
		/// <exception cref="TransactionFailureException">Thrown if a critical error occurred determining a transaction status and the user must be prompted to provide the result instead.</exception>
		public async Task<TResponseMessage> ProcessRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage) where TRequestMessage : PosLinkRequestMessageBase where TResponseMessage : PosLinkResponseMessageBase
		{
			requestMessage.GuardNull(nameof(requestMessage));
			requestMessage.Validate();

			//TODO: Handle connection failure when someone else is connected (socket error, connected refused).
			OnDisplayMessage(new DisplayMessage(StatusMessages.Connecting, DisplayMessageSource.Library));
			using (var connection = await ConnectAsync(_Address, _Port).ConfigureAwait(false))
			{
				OnDisplayMessage(new DisplayMessage(StatusMessages.CheckingDeviceStatus, DisplayMessageSource.Library));
				await ConfirmDeviceNotBusy(connection).ConfigureAwait(false);
				OnDisplayMessage(new DisplayMessage(StatusMessages.SendingRequest, DisplayMessageSource.Library));
				await SendAndWaitForAck(requestMessage, connection).ConfigureAwait(false);
				OnDisplayMessage(new DisplayMessage(StatusMessages.WaitingForResponse, DisplayMessageSource.Library));
				return await _Reader.ReadMessageAsync<TResponseMessage>(connection.InStream, connection.OutStream, OnDisplayMessage).ConfigureAwait(false);
			}
		}

		private void OnDisplayMessage(DisplayMessage message)
		{
			try
			{
				DisplayMessage?.Invoke(this, new DisplayMessageEventArgs(message));
			}
			catch
			{
				//TODO: Logging
#if DEBUG
				throw;
#endif
			}
		}
		private async Task SendAndWaitForAck<TRequest>(TRequest requestMessage, PinpadConnection connection) where TRequest : PosLinkRequestMessageBase
		{
			var retries = 0;
			while (retries < ProtocolConstants.MaxRetries)
			{
				await _Writer.WriteMessageAsync(requestMessage, connection.OutStream).ConfigureAwait(false);
				try
				{
					await _Reader.WaitForAck(connection.InStream).ConfigureAwait(false);
					return;
				}
				catch (PosLinkNackException nex)
				{
					// Spec 2.2, Page 7; For any NAK the sender retries a maximum of 3 times. 
					retries++;
					if (retries >= ProtocolConstants.MaxRetries)
					{
						throw new TransactionFailureException(ErrorMessages.TransactionFailure, nex);
					}

					await Task.Delay(ProtocolConstants.RetryDelay_Milliseconds).ConfigureAwait(false);
				}
			}

			//Should never get here but keeps compiler happy.
			throw new TransactionFailureException(ErrorMessages.TransactionFailure, new PosLinkNackException());
		}

		private async Task ConfirmDeviceNotBusy(PinpadConnection connection)
		{
			var message = new PollRequestMessage();

			await SendAndWaitForAck<PollRequestMessage>(message, connection).ConfigureAwait(false);

			var response = await _Reader.ReadMessageAsync<PollResponseMessage>(connection.InStream, connection.OutStream, OnDisplayMessage).ConfigureAwait(false);
			if (response.Status == DeviceStatus.Ready) return;

			throw new DeviceBusyException(String.IsNullOrWhiteSpace(response.Display) ? ErrorMessages.TerminalBusy : response.Display);
		}

		private async Task<PinpadConnection> ConnectAsync(string address, int port)
		{
			var socket = new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.IP);
			try
			{
				//TODO: Change connect to async
				socket.Connect(address, port);
				var connection = new PinpadConnection()
				{
					Socket = socket
				};
				await connection.ClearInputBuffer().ConfigureAwait(false);

				return connection;
			}
			catch
			{
				socket?.Dispose();
				throw;
			}
		}
	}
}