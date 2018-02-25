using Ladon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
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

		private object _Synchroniser;
		private readonly MessageWriter _Writer;
		private readonly MessageReader _Reader;

		private PinpadConnection _CurrentConnection;
		private Task<PosLinkResponseBase> _CurrentReadTask;
		private PosLinkRequestBase _LastRequest;
		private int _CurrentRequestMerchant;

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
			_Synchroniser = new object();
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
		/// <exception cref="DeviceBusyException">Thrown if the device is already processing a request.</exception>
		public async Task<TResponseMessage> ProcessRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage) 
			where TRequestMessage : PosLinkRequestBase 
			where TResponseMessage : PosLinkResponseBase
		{
			requestMessage.GuardNull(nameof(requestMessage));
			requestMessage.Validate();

			GlobalSettings.Logger.LogInfo(String.Format(LogMessages.ProcessRequest, requestMessage.MerchantReference, requestMessage.RequestType));

			PinpadConnection existingConnection = null;
			lock (_Synchroniser)
			{
				if (_CurrentConnection != null && requestMessage.RequestType != ProtocolConstants.MessageType_Cancel)
					throw new DeviceBusyException();

				existingConnection = _CurrentConnection;
			}

			try
			{
				OnDisplayMessage(new DisplayMessage(StatusMessages.Connecting, DisplayMessageSource.Library));
				using (var connection = await ConnectAsync(_Address, _Port).ConfigureAwait(false))
				{
					return await SendAndWaitForResponseWithRetriesAsync<TRequestMessage, TResponseMessage>(requestMessage, existingConnection, connection).ConfigureAwait(false);
				}
			}
			finally
			{
				lock (_Synchroniser)
				{
					_CurrentReadTask = null;
					_CurrentConnection = null;
					_CurrentRequestMerchant = 0;
				}
			}
		}

		private async Task<TResponseMessage> SendAndWaitForResponseWithRetriesAsync<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage, PinpadConnection existingConnection, PinpadConnection connection)
			where TRequestMessage : PosLinkRequestBase
			where TResponseMessage : PosLinkResponseBase
		{
			var retry = 0;
			while (retry <= ProtocolConstants.MaxRetries)
			{
				if (existingConnection == connection)
				{
					//Special handling as connection already open and already
					//a task reading incoming data. Cancellation is the only request 
					//we should process while another request is being processed.
					_LastRequest = requestMessage;
					OnDisplayMessage(new DisplayMessage(StatusMessages.SendingRequest, DisplayMessageSource.Library));
					await _Writer.WriteMessageAsync<TRequestMessage>(requestMessage, connection.OutStream).ConfigureAwait(false);
				}
				else
				{
					if (retry == 0)
						_CurrentRequestMerchant = requestMessage.Merchant;

					OnDisplayMessage(new DisplayMessage(StatusMessages.CheckingDeviceStatus, DisplayMessageSource.Library));
					try
					{
						var pollResponse = await PollDeviceStatus(connection, retry == 0).ConfigureAwait(false); // If this is not the first attempt we just want to know the device is responding at all
						//If we were only asked to poll, just return the response we already have.
						if (requestMessage.RequestType == ProtocolConstants.MessageType_Poll)
							return (TResponseMessage)(PosLinkResponseBase)pollResponse;
					}
					catch (DeviceBusyException dbe)
					{
						throw new TransactionFailureException(ErrorMessages.TransactionFailure, dbe);
					}

					OnDisplayMessage(new DisplayMessage(StatusMessages.SendingRequest, DisplayMessageSource.Library));
					await SendAndWaitForAck(requestMessage, connection).ConfigureAwait(false);
				}

				try
				{
					OnDisplayMessage(new DisplayMessage(StatusMessages.WaitingForResponse, DisplayMessageSource.Library));
					if (_CurrentReadTask == null)
						_CurrentReadTask = ReadUntilFinalResponse<TResponseMessage>(connection);

					var retVal = await _CurrentReadTask.ConfigureAwait(false);
					return (TResponseMessage)retVal;
				}
				catch (OperationCanceledException)
				{
					retry++;
				}
			}

			//After max retries we still got no response
			//See POS Link spec 2.2, page 45, Messaging Timeouts section.
			throw new TransactionFailureException(ErrorMessages.TransactionFailure);
		}

		private async Task<PosLinkResponseBase> ReadUntilFinalResponse<TResponseMessage>(PinpadConnection connection) where TResponseMessage : PosLinkResponseBase
		{
			TResponseMessage retVal = null;
			while (retVal == null)
			{
				PosLinkResponseBase message = null;

				var retries = 0;
				while (retries < ProtocolConstants.MaxRetries)
				{
					try
					{
						message = await _Reader.ReadMessageAsync(connection.InStream, connection.OutStream).ConfigureAwait(false);
						break;
					}
					catch (PosLinkNackException)
					{
						retries++;
						if (_LastRequest == null || retries >= ProtocolConstants.MaxRetries)
							throw;

						await Task.Delay(ProtocolConstants.ReadDelay_Milliseconds).ConfigureAwait(false);

						await SendAndWaitForAck(_LastRequest, connection).ConfigureAwait(false);
					}
				}

				retVal = message as TResponseMessage;
				if (retVal != null) break;

				switch (message.MessageType)
				{
					case ProtocolConstants.MessageType_Display:
						OnDisplayMessage(new DisplayMessage(((DisplayMessageResponse)message).MessageText, DisplayMessageSource.Pinpad));
						break;

					case ProtocolConstants.MessageType_Ask:
						await ProcessAskRequest((AskRequest)message, connection).ConfigureAwait(false);
						break;

					case ProtocolConstants.MessageType_Sig:
						await ProcessSigRequest((SignatureRequest)message, connection).ConfigureAwait(false);
						break;

					case ProtocolConstants.MessageType_Error:
						var errorResponse = message as ErrorResponse;
						throw new PosLinkProtocolException(errorResponse.Display, errorResponse.Response);

					default:
						throw new UnexpectedResponseException(message);
				}
			}

			return retVal;
		}

		private async Task ProcessAskRequest(AskRequest request, PinpadConnection connection)
		{
			var responseValue = await OnQueryOperator(request.MerchantReference, request.Prompt, null, ProtocolConstants.DefaultQueryResponses).ConfigureAwait(false);
			if (responseValue == null) return;

			var responseMessage = new AskResponse()
			{
				MerchantReference = request.MerchantReference,
				Merchant = _CurrentRequestMerchant == 0 ? GlobalSettings.DefaultMerchant : _CurrentRequestMerchant,
				Response = responseValue
			};
			await SendAndWaitForAck(responseMessage, connection).ConfigureAwait(false);
		}

		private async Task ProcessSigRequest(SignatureRequest request, PinpadConnection connection)
		{
			var responseValue = await OnQueryOperator(request.MerchantReference, request.Prompt, request.ReceiptText, ProtocolConstants.DefaultQueryResponses).ConfigureAwait(false);
			if (responseValue == null) return;

			var responseMessage = new SignatureResponse()
			{
				MerchantReference = request.MerchantReference,
				Merchant = _CurrentRequestMerchant == 0 ? GlobalSettings.DefaultMerchant : _CurrentRequestMerchant,
				Response = responseValue
			};
			await SendAndWaitForAck(responseMessage, connection).ConfigureAwait(false);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void OnDisplayMessage(DisplayMessage message)
		{
			try
			{
				GlobalSettings.Logger.LogInfo(String.Format(System.Globalization.CultureInfo.InvariantCulture, LogMessages.DisplayMessage, message.Source, message.MessageText));

				DisplayMessage?.Invoke(this, new DisplayMessageEventArgs(message));
			}
			catch (Exception ex)
			{
				GlobalSettings.Logger.LogWarn(LogMessages.ErrorInDisplayMessageEvent, ex);
#if DEBUG
				throw;
#endif
			}
		}

		private async Task<string> OnQueryOperator(string merchantReference, string prompt, string receiptText, IReadOnlyList<string> allowedResponses)
		{
			GlobalSettings.Logger.LogInfo(String.Format(LogMessages.QueryOperator, merchantReference, prompt, receiptText, String.Join(", ", allowedResponses)));

			var eventArgs = new QueryOperatorEventArgs(prompt, receiptText, allowedResponses);
			if (QueryOperator == null) throw new InvalidOperationException(ErrorMessages.NoHandlerForQueryOperatorConnected);

			QueryOperator?.Invoke(this, eventArgs);

			var retVal = await eventArgs.ResponseTask.ConfigureAwait(false);
			GlobalSettings.Logger.LogInfo(String.Format(LogMessages.QueryOperatorResponse, merchantReference, retVal));

			return retVal;
		}

		private async Task SendAndWaitForAck(PosLinkRequestBase requestMessage, PinpadConnection connection) 
		{
			var retries = 0;
			while (retries < ProtocolConstants.MaxRetries)
			{
				GlobalSettings.Logger.LogInfo(String.Format(LogMessages.SendingRequest, requestMessage.RequestType, requestMessage.MerchantReference));

				await _Writer.WriteMessageAsync(requestMessage, connection.OutStream).ConfigureAwait(false);
				try
				{
					await MessageReader.WaitForAck(connection.InStream).ConfigureAwait(false);
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

		private async Task<PollResponse> PollDeviceStatus(PinpadConnection connection, bool throwIfBusy)
		{
			var message = new PollRequest() { Merchant = _CurrentRequestMerchant == 0 ? GlobalSettings.DefaultMerchant : _CurrentRequestMerchant };

			await SendAndWaitForAck(message, connection).ConfigureAwait(false);

			var retVal = (PollResponse)(await ReadUntilFinalResponse<PollResponse>(connection).ConfigureAwait(false));

			if (retVal.Status == DeviceStatus.Busy)
				throw new DeviceBusyException(String.IsNullOrWhiteSpace(retVal.Display) ? ErrorMessages.TerminalBusy : retVal.Display);

			return retVal;
		}

		private async Task<PinpadConnection> ConnectAsync(string address, int port)
		{
			lock (_Synchroniser)
			{
				if (_CurrentConnection != null) return _CurrentConnection;
			}

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

				return _CurrentConnection = connection;
			}
			catch (System.Net.Sockets.SocketException sex)
			{
				if (ErrorCodeIndicatesBusy(sex.SocketErrorCode)) throw new DeviceBusyException(sex.Message, sex);
				throw;
			}
			catch
			{
				socket?.Dispose();
				throw;
			}
		}

		private static bool ErrorCodeIndicatesBusy(SocketError socketErrorCode)
		{
			return socketErrorCode == SocketError.AlreadyInProgress
				|| socketErrorCode == SocketError.ConnectionRefused
				|| socketErrorCode == SocketError.InProgress
				|| socketErrorCode == SocketError.IsConnected;
		}
	}
}