using Ladon;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// The main class used for communicating with a Verifone Pin Pad via the POS Link protocol.
	/// </summary>
	/// <remarks>
	/// <para>Send requests (and receive responses) voa the <see cref="ProcessRequest{TRequest, TResponse}(TRequest)"/> method.</para>
	/// </remarks>
	/// <seealso cref="IPinpadClient"/>
	public class PinpadClient : IPinpadClient
	{

		#region Fields

		private readonly string _Address;
		private readonly int _Port;

		private object _Synchroniser;
		private readonly MessageWriter _Writer;
		private readonly MessageReader _Reader;

		private PinpadConnection _CurrentConnection;
		private Task<PosLinkResponseBase> _CurrentReadTask;

		//Used to resend the last message sent to the pinpad in the case of an ack failure,
		//required as sometimes a different thread is responsible for writing than reading.
		private PosLinkRequestBase _LastRequest;
		private string _CurrentMerchantReference;
		private int _CurrentRequestMerchant;

		#endregion

		#region Events

		/// <summary>
		/// Raised when there is an information prompt or status change that should be displayed to the user.
		/// </summary>
		/// <remarks>
		/// <para>This event may be raised from background threads, any event handler code interacting with UI may need to invoke to the UI thread.</para>
		/// </remarks>
		public event EventHandler<DisplayMessageEventArgs> DisplayMessage;
		/// <summary>
		/// Raised when there is a question that must be answered by the operator.
		/// </summary>
		/// <remarks>
		/// <para>This event may be raised from background threads, any event handler code interacting with UI may need to invoke to the UI thread.</para>
		/// </remarks>
		public event EventHandler<QueryOperatorEventArgs> QueryOperator;

		#endregion

		#region Constructors

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
			_Reader.AcknowledgmentTimeout += Reader_AcknowledgmentTimeout;
		}

		private void Reader_AcknowledgmentTimeout(object sender, EventArgs e)
		{
			OnDisplayMessage(new PosLink.DisplayMessage(_CurrentMerchantReference, ErrorMessages.TerminalBusy, DisplayMessageSource.Library));
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sends a request to the pin pad and returns the response. If the device is already busy or processing a request, throws a <see cref="DeviceBusyException"/>.
		/// </summary>
		/// <typeparam name="TRequestMessage">The type of request to send.</typeparam>
		/// <typeparam name="TResponseMessage">The type of response expected.</typeparam>
		/// <param name="requestMessage">The request message to be sent.</param>
		/// <returns>A instance of {TResponseMessage} containing the pin pad response.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestMessage"/> message is null, or if any required property of the request is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if any property of <paramref name="requestMessage"/> is invalid.</exception>
		/// <exception cref="TransactionFailureException">Thrown if a critical error occurred determining a transaction status and the user must be prompted to provide the result instead.</exception>
		/// <exception cref="DeviceBusyException">Thrown if the device is already processing a request or does not respond to the request.</exception>
		public async Task<TResponseMessage> ProcessRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage)
			where TRequestMessage : PosLinkRequestBase
			where TResponseMessage : PosLinkResponseBase
		{
			return await ProcesssRequest<TRequestMessage, TResponseMessage>(requestMessage, true).ConfigureAwait(false);
		}

		/// <summary>
		/// Sends a request to the pin pad and returns the response. Intended to be used for retrying requests that have already been sent, does not check if the pinpad is already busy.
		/// </summary>
		/// <typeparam name="TRequestMessage">The type of request to send.</typeparam>
		/// <typeparam name="TResponseMessage">The type of response expected.</typeparam>
		/// <param name="requestMessage">The request message to be sent.</param>
		/// <returns>A instance of {TResponseMessage} containing the pin pad response.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="requestMessage"/> message is null, or if any required property of the request is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if any property of <paramref name="requestMessage"/> is invalid.</exception>
		/// <exception cref="TransactionFailureException">Thrown if a critical error occurred determining a transaction status and the user must be prompted to provide the result instead.</exception>
		/// <exception cref="DeviceBusyException">Thrown if the device does not respond to the request.</exception>
		public async Task<TResponseMessage> RetryRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage)
			where TRequestMessage : PosLinkRequestBase
			where TResponseMessage : PosLinkResponseBase
		{
			return await ProcesssRequest<TRequestMessage, TResponseMessage>(requestMessage, true).ConfigureAwait(false);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Sets or returns a value indicating how to handle a communication failure with a pinpad device that means a transaction result cannot be determined.
		/// </summary>
		/// <remarks>
		/// <para>See the <see cref="TransactionFailureHandlingStrategy"/> for details on the possible options.</para>
		/// </remarks>
		public TransactionFailureHandlingStrategy TransactionFailureHandlingStrategy { get; set; }

		#endregion

		#region Private Methods

		private async Task<TResponseMessage> ProcesssRequest<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage, bool isNewRequest)
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

			try // Ensure state clean up
			{
				try //Handle final comms failure
				{
					_CurrentRequestMerchant = requestMessage.Merchant;
					_CurrentMerchantReference = requestMessage.MerchantReference;

					var haveConnectedAtLeastOnce = false;
					var retries = 0;
					while (retries < ProtocolConstants.Max_ConnectionRetries)
					{
						if (retries > 0)
							GlobalSettings.Logger.LogWarn(LogMessages.AttemptingReconnect);

						try // Handle retryable connection errors/timeouts 
						{
							if (existingConnection == null)
								OnDisplayMessage(new DisplayMessage(requestMessage.MerchantReference, StatusMessages.Connecting, DisplayMessageSource.Library));

							using (var connection = await ConnectAsync(_Address, _Port).ConfigureAwait(false))
							{
								if (!haveConnectedAtLeastOnce)
								{
									haveConnectedAtLeastOnce = true;
									retries = 0;
								}
								return await SendAndWaitForFinalResponseAsync<TRequestMessage, TResponseMessage>(requestMessage, existingConnection, connection, isNewRequest && !haveConnectedAtLeastOnce).ConfigureAwait(false);
							}
						}
						catch (TransactionFailureException)
						{
							throw;
						}
						catch (ArgumentException)
						{
							throw;
						}
						catch (Exception ex)
						{
							if (ex as TimeoutException != null && haveConnectedAtLeastOnce)
								GlobalSettings.Logger.LogWarn(LogMessages.NoResponseFromDeviceRetry, ex);
							else
								GlobalSettings.Logger.LogError(ex.Message, ex);	

							if (!haveConnectedAtLeastOnce) throw new PosLinkConnectionException(ErrorMessages.ConnectionFailed, ex);
						}

						retries++;

						// Reset connection before trying again.
						try
						{
							_CurrentConnection?.Dispose();
						}
						catch (ObjectDisposedException) { }
						_CurrentConnection = null;
						_CurrentReadTask = null;
					}

					throw new TransactionFailureException();
				}
				catch (TransactionFailureException te)
				{
					GlobalSettings.Logger.LogError(te.Message, te);
					//See POS Link spec 2.2, page 45, Messaging Timeouts section.
					//If we got no response after a predetermined time (advised 60 seconds) 
					//then request result from user.
					var tranRequest = requestMessage as PosLinkTransactionOptionsRequestBase;
					if (this.TransactionFailureHandlingStrategy == TransactionFailureHandlingStrategy.ThrowException || tranRequest == null) throw;

					return await HandleTransactionFailure<TResponseMessage>(tranRequest).ConfigureAwait(false);
				}
			}
			finally
			{
				lock (_Synchroniser)
				{
					_CurrentReadTask = null;
					_CurrentConnection = null;
					_CurrentRequestMerchant = 0;
					_CurrentMerchantReference = null;
				}
			}
		}

		private async Task<TResponseMessage> HandleTransactionFailure<TResponseMessage>(PosLinkTransactionOptionsRequestBase request) where TResponseMessage : PosLinkResponseBase
		{
			var userResponse = await OnQueryOperator(request.MerchantReference, ErrorMessages.TransactionFailure, null, ProtocolConstants.DefaultQueryResponses).ConfigureAwait(false);
			var accepted = userResponse == ProtocolConstants.Response_Yes;

			var f = new ResponseMessageFactory();
			return (TResponseMessage)(PosLinkResponseBase)f.CreateManualTransactionResponse(request, accepted);
		}

		private async Task<TResponseMessage> SendAndWaitForFinalResponseAsync<TRequestMessage, TResponseMessage>(TRequestMessage requestMessage, PinpadConnection existingConnection, PinpadConnection connection, bool isNewRequest)
			where TRequestMessage : PosLinkRequestBase
			where TResponseMessage : PosLinkResponseBase
		{
			try
			{
				var isCancelRequest = requestMessage.RequestType == ProtocolConstants.MessageType_Cancel;

				if (!isCancelRequest || _CurrentReadTask == null)
				{
					var pollResponse = await PollDeviceStatusAsync(connection).ConfigureAwait(false);

					// Both new requests and re-requests require a poll according to the spec,
					// it's just how we handle the result that changes
					if (requestMessage.RequestType == ProtocolConstants.MessageType_Poll) //Client asked for poll so just send this response, no need to poll a second time.
						return (TResponseMessage)(PosLinkResponseBase)pollResponse;
					else if (pollResponse.Status == DeviceStatus.Ready && isCancelRequest) // We're not busy and they asked to cancel, which is an error (otherwise client waits for a response that will never come)
						throw new NoTransactionInProgressException();
					else if (pollResponse.Status == DeviceStatus.Busy && isNewRequest && !isCancelRequest) // Pinpad is busy processing a request and this is a *new* request so error as this sort of concurrency is not supported by device.
						throw new DeviceBusyException();
				}
			}
			catch (Exception ex)
			{
				if (isNewRequest) throw;

				throw new TransactionFailureException(ErrorMessages.TransactionFailure, ex);
			}

			if (existingConnection == connection)
			{
				//Special handling as connection already open and there is already
				//a task reading incoming data running. We don't want to start a second read.
				//Cancellation is the only request we should process while another request is being processed.
				_LastRequest = requestMessage;
				OnDisplayMessage(new DisplayMessage(requestMessage.MerchantReference, StatusMessages.SendingRequest, DisplayMessageSource.Library));
				await _Writer.WriteMessageAsync<TRequestMessage>(requestMessage, connection.OutStream).ConfigureAwait(false);
			}
			else
			{
				OnDisplayMessage(new DisplayMessage(requestMessage.MerchantReference, StatusMessages.SendingRequest, DisplayMessageSource.Library));
				await SendAndWaitForAck(requestMessage, connection).ConfigureAwait(false);
			}

			OnDisplayMessage(new DisplayMessage(requestMessage.MerchantReference, StatusMessages.WaitingForResponse, DisplayMessageSource.Library));
			if (_CurrentReadTask == null)
				_CurrentReadTask = ReadUntilFinalResponse<TResponseMessage>(requestMessage.MerchantReference, connection);

			var retVal = await _CurrentReadTask.ConfigureAwait(false);

			//Spec 2.2, page 46
			if ((requestMessage.RequestType != ProtocolConstants.MessageType_Cancel && retVal.MessageType != requestMessage.RequestType) || retVal.MerchantReference != requestMessage.MerchantReference) throw new UnexpectedResponseException(retVal);

			return (TResponseMessage)retVal;
		}

		private async Task<PosLinkResponseBase> ReadUntilFinalResponse<TResponseMessage>(string requestReference, PinpadConnection connection) where TResponseMessage : PosLinkResponseBase
		{
			TResponseMessage retVal = null;
			while (retVal == null)
			{
				PosLinkResponseBase message = null;

				var retries = 0;
				while (retries < ProtocolConstants.MaxNackRetries)
				{
					try
					{
						message = await _Reader.ReadMessageAsync(connection.InStream, connection.OutStream).ConfigureAwait(false);
						break;
					}
					catch (PosLinkNackException)
					{
						retries++;
						if (_LastRequest == null || retries > ProtocolConstants.MaxNackRetries)
							throw;

						await Task.Delay(ProtocolConstants.ReadDelay_Milliseconds).ConfigureAwait(false);

						await SendAndWaitForAck(_LastRequest, connection).ConfigureAwait(false);
					}
				}

				//Poll is an exception because it's id is always XXXXXX apparently.
				//Spec 2.2, page 46
				if (message.MerchantReference != requestReference && message.MessageType != ProtocolConstants.MessageType_Poll) throw new UnexpectedResponseException(message);

				retVal = message as TResponseMessage;
				if (retVal != null) break;
				switch (message.MessageType)
				{
					case ProtocolConstants.MessageType_Display:
						OnDisplayMessage(new DisplayMessage(message.MerchantReference, ((DisplayMessageResponse)message).MessageText, DisplayMessageSource.Pinpad));
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

					case ProtocolConstants.MessageType_Poll: //If we got a poll response, that's cool, but just keep waiting for something to do
						break;

					default:
						throw new UnexpectedResponseException(message); //Spec 2.2, page 46
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

			var eventArgs = new QueryOperatorEventArgs(merchantReference, prompt, receiptText, allowedResponses);
			if (QueryOperator == null) throw new InvalidOperationException(ErrorMessages.NoHandlerForQueryOperatorConnected);

			QueryOperator?.Invoke(this, eventArgs);

			var retVal = await eventArgs.ResponseTask.ConfigureAwait(false);
			GlobalSettings.Logger.LogInfo(String.Format(LogMessages.QueryOperatorResponse, merchantReference, retVal));

			return retVal;
		}

		private async Task SendAndWaitForAck(PosLinkRequestBase requestMessage, PinpadConnection connection)
		{
			var retries = 0;
			while (retries < ProtocolConstants.MaxNackRetries)
			{
				GlobalSettings.Logger.LogInfo(String.Format(LogMessages.SendingRequest, requestMessage.RequestType, requestMessage.MerchantReference));

				await _Writer.WriteMessageAsync(requestMessage, connection.OutStream).ConfigureAwait(false);
				_LastRequest = requestMessage;
				try
				{
					await _Reader.WaitForAck(connection.InStream).ConfigureAwait(false);
					return;
				}
				catch (PosLinkNackException nex)
				{
					// Spec 2.2, Page 7; For any NAK the sender retries a maximum of 3 times. 
					GlobalSettings.Logger.LogWarn(LogMessages.ReceivedNack, nex);

					retries++;
					if (retries >= ProtocolConstants.MaxNackRetries) throw;

					await Task.Delay(ProtocolConstants.RetryDelay_Milliseconds).ConfigureAwait(false);
				}
			}

			//Should never get here but keeps compiler happy.
			throw new TransactionFailureException(ErrorMessages.TransactionFailure, new PosLinkNackException());
		}

		private async Task<PollResponse> PollDeviceStatusAsync(PinpadConnection connection)
		{
			var message = new PollRequest() { Merchant = _CurrentRequestMerchant == 0 ? GlobalSettings.DefaultMerchant : _CurrentRequestMerchant };

			await SendAndWaitForAck(message, connection).ConfigureAwait(false);

			return (PollResponse)(await ReadUntilFinalResponse<PollResponse>(message.MerchantReference, connection).ConfigureAwait(false));
		}

		private Task<PinpadConnection> ConnectAsync(string address, int port)
		{
			lock (_Synchroniser)
			{
				if (_CurrentConnection != null) return Task.FromResult(_CurrentConnection);
			}

			var socket = new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.IP);
			try
			{
				PinpadConnection connection = null;
				var connectTcs = new System.Threading.Tasks.TaskCompletionSource<PinpadConnection>();
				var args = new SocketAsyncEventArgs();
				
				EventHandler<SocketAsyncEventArgs> socketConnectedHandler = null;
				socketConnectedHandler = (EventHandler<SocketAsyncEventArgs>)
				(
					async (sender, e) =>
					{
						try
						{
							e.Completed -= socketConnectedHandler;

							//Do not dispose 'e', doing so will close socket.

							if (e.SocketError != SocketError.Success || !(e.ConnectSocket?.Connected ?? false))
							{
								if (ErrorCodeIndicatesBusy(e.SocketError))
								{
									var sex = new SocketException((int)e.SocketError);
									connectTcs.TrySetException(new DeviceBusyException(sex.Message, sex));
								}
								else
									connectTcs.TrySetException(new SocketException((int)e.SocketError));
								return;
							}

							e.ConnectSocket.NoDelay = true;
							connection = new PinpadConnection()
							{
								Socket = e.ConnectSocket
							};

							await connection.ClearInputBuffer().ConfigureAwait(false);

							_CurrentConnection = connection;
							connectTcs.TrySetResult(connection);
						}
						catch (Exception ex)
						{
							e.Dispose();
							connectTcs.TrySetException(ex);
						}
					}
				);

				args.Completed += socketConnectedHandler;
				args.RemoteEndPoint = GetSocketEndpoint(address, port);

				socket.LingerState = new LingerOption(false, 0);
				socket.ExclusiveAddressUse = false;
				socket.ConnectAsync(args);

				return connectTcs.Task;
			}
			catch
			{
				socket?.Dispose();
				throw;
			}
		}

		private static EndPoint GetSocketEndpoint(string address, int port)
		{
			if (IPAddress.TryParse(address, out var ipAddress))
				return new IPEndPoint(ipAddress, port);

			return new DnsEndPoint(address, port);
		}

		private static bool ErrorCodeIndicatesBusy(SocketError socketErrorCode)
		{
			return socketErrorCode == SocketError.AlreadyInProgress
				|| socketErrorCode == SocketError.ConnectionRefused
				|| socketErrorCode == SocketError.InProgress
				|| socketErrorCode == SocketError.IsConnected;
		}

		#endregion

	}
}