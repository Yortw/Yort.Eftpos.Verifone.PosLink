using Ladon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// A class that reads raw data from a stream, parse and decodes the values then deserialises the result into a type derived from <see cref="PosLinkResponseMessageBase"/>.
	/// </summary>
	public class MessageReader
	{
		private System.Text.ASCIIEncoding _Encoding;
		private ResponseMessageFactory _MessageFactory;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MessageReader()
		{
			_MessageFactory = new ResponseMessageFactory();
			_Encoding = new ASCIIEncoding();
		}

		/// <summary>
		/// Waits for an ACK response to arrive in the stream.
		/// </summary>
		/// <param name="inStream">The stream to read from.</param>
		/// <returns>A <see cref="System.Threading.Tasks.Task"/> that can be waited on to determine when an ack has arrived.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="inStream"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="inStream"/> is not readable.</exception>
		public async Task WaitForAck(System.IO.Stream inStream)
		{
			inStream.GuardNull(nameof(inStream));
			if (!inStream.CanRead) throw new ArgumentException(ErrorMessages.StreamMustBeReadable, nameof(inStream));

			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			using (var cancelTokenSource = new System.Threading.CancellationTokenSource())
			{
				cancelTokenSource.CancelAfter(ProtocolConstants.Timeout_ReceiveAck_Milliseconds);

				while (sw.ElapsedMilliseconds < ProtocolConstants.Timeout_ReceiveAck_Milliseconds)
				{
					try
					{
						using (var dataBuffer = await ReadData(inStream, 1, cancelTokenSource.Token).ConfigureAwait(false))
						{
							for (var cnt = 0; cnt < (dataBuffer?.Length ?? 0); cnt++)
							{
								if (dataBuffer.Bytes[cnt] == ProtocolConstants.ControlByte_Ack) return;
								if (dataBuffer.Bytes[cnt] == ProtocolConstants.ControlByte_Nack) throw new PosLinkNackException();
							}
						}

						await Task.Delay(ProtocolConstants.ReadDelay_Milliseconds).ConfigureAwait(false);
					}
					catch (TaskCanceledException tce)
					{
						throw new DeviceBusyException(ErrorMessages.TerminalBusy, tce);
					}
				}
				sw.Stop();

				//Spec 2.2, Page7;
				//Any failure to receive a response/ACK from the terminal, the POS should assume the device is busy 
				//processing and report accordingly. 
				//Multiple transmissions should not be attempted. 
				throw new DeviceBusyException();
			}
		}

		/// <summary>
		/// Reads from the stream until a message of type {TResponseMessage} is received.
		/// </summary>
		/// <typeparam name="TResponseMessage">The type of response message to wait for.</typeparam>
		/// <param name="inStream">The stream to read from.</param>
		/// <param name="outStream">The stream to write acknowledgements to.</param>
		/// <returns>A task whose result is an object of the requested {TResponseMessage} type.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="inStream"/> or <paramref name="outStream"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="inStream"/> is not readable or <paramref name="outStream"/> is not writeable.</exception>
		public async Task<TResponseMessage> ReadMessageAsync<TResponseMessage>(System.IO.Stream inStream, System.IO.Stream outStream) where TResponseMessage : PosLinkResponseMessageBase
		{
			inStream.GuardNull(nameof(inStream));
			if (!inStream.CanRead) throw new ArgumentException(ErrorMessages.StreamMustBeReadable, nameof(inStream));
			outStream.GuardNull(nameof(outStream));
			if (!outStream.CanWrite) throw new ArgumentException(ErrorMessages.StreamMustBeWriteable, nameof(outStream));

			Task sendAckTask = null;
			var waitingForResponse = true;
			while (waitingForResponse)
			{
				if (sendAckTask != null)
					await sendAckTask.ConfigureAwait(false);

				using (var messageBuffer = await ReadMessageBytes(inStream).ConfigureAwait(false))
				{
					var fieldValues = ValidateMessageAndParseFields(messageBuffer);


					sendAckTask = SendAckAsync(outStream);

					System.Diagnostics.Debug.WriteLine("Received " + fieldValues[0] + " " + fieldValues[1]);
					if (fieldValues[1] == ProtocolConstants.MessageType_Display)
						System.Diagnostics.Debug.WriteLine(fieldValues[3]);

					var message = _MessageFactory.CreateMessage(fieldValues);
					if (message is TResponseMessage retVal)
						return retVal;

					if (message == null) continue; //Unknown message type, keep going

					if (message.MessageType == ProtocolConstants.MessageType_Poll)
						continue;
					else if (message.MessageType == ProtocolConstants.MessageType_Display)
					{
						//TODO: This.
						throw new NotImplementedException();
					}
					else if (message.MessageType == ProtocolConstants.MessageType_Error)
					{
						//TODO: Properly handle error.
						throw new PosLinkProtocolException();
					}
				}
			}

			//TODO: Properly handle error. Should never get here?
			throw new PosLinkProtocolException();
		}

		private async Task SendAckAsync(Stream outStream)
		{
			outStream.WriteByte(ProtocolConstants.ControlByte_Ack);
			await outStream.FlushAsync().ConfigureAwait(false);
		}

		private async Task<DataBuffer> ReadMessageBytes(System.IO.Stream inStream)
		{
			using (var cancelTokenSource = new System.Threading.CancellationTokenSource())
			{
				cancelTokenSource.CancelAfter(ProtocolConstants.Timeout_ReadResponse_Milliseconds);

				try
				{
					DataBuffer retVal = null;
					while (retVal == null) 
					{
						retVal = await ReadData(inStream, ProtocolConstants.MaxBufferSize_Read, cancelTokenSource.Token).ConfigureAwait(false);
						if (retVal?.Length < ProtocolConstants.ValidMesage_MinBytes)
						{
							//TODO: Report this somehow? Stop reading?
							//Invalid message
							retVal = null;
						}
						cancelTokenSource.Token.ThrowIfCancellationRequested();
					}
					return retVal;
				}
				catch (OperationCanceledException oce)
				{
					throw new DeviceBusyException(ErrorMessages.TerminalBusy, oce);
				}

				//Spec 2.2, Page7;
				//Any failure to receive a response/ACK from the terminal, the POS should assume the device is busy 
				//processing and report accordingly. 
				//Multiple transmissions should not be attempted. 
				throw new DeviceBusyException();
			}
		}
		
		private async Task<DataBuffer> ReadData(System.IO.Stream inStream, int maxBytesToRead, System.Threading.CancellationToken cancelToken)
		{
			//TODO: Recycle buffer
			//TODO: Handle left over bytes from previous read (prefix to this result)?
			var buffer = new byte[maxBytesToRead];
			var bytesActuallyRead = await inStream.ReadAsync(buffer, 0, maxBytesToRead, cancelToken).ConfigureAwait(false);
			if (bytesActuallyRead == 0) return null;

			return new DataBuffer(new ArraySegment<byte>(buffer, 0, bytesActuallyRead));
		}

		private IList<string> ValidateMessageAndParseFields(DataBuffer messageBuffer)
		{
			if (messageBuffer.Bytes[0] != ProtocolConstants.ControlByte_Stx) throw new PosLinkProtocolException(ErrorMessages.MessageDoesNotStartWithStx);
			if (messageBuffer.Bytes[messageBuffer.Length - 2] != ProtocolConstants.ControlByte_Etx) throw new PosLinkProtocolException(ErrorMessages.MessageDoesNotEndWithEtx);
			if (!IsLrcValid(messageBuffer)) throw new PosLinkProtocolException(ErrorMessages.MessageLrcInvalid);

			var retVal = new List<string>(10);

			var fieldStartIndex = 1;
			for (var idx = 1; idx < messageBuffer.Length - 2; idx++)
			{
				if (messageBuffer.Bytes[idx] == ProtocolConstants.ControlByte_Comma)
				{
					var value = ProtocolUtilities.DecodeSpecialCharacters(_Encoding.GetString(messageBuffer.Bytes, fieldStartIndex, idx - fieldStartIndex));
					retVal.Add(value);
					fieldStartIndex = idx + 1;
				}
			}

			if (fieldStartIndex < messageBuffer.Length - 2)
				retVal.Add(ProtocolUtilities.DecodeSpecialCharacters(_Encoding.GetString(messageBuffer.Bytes, fieldStartIndex, (messageBuffer.Length - 2) - fieldStartIndex)));

			return retVal;
		}

		private bool IsLrcValid(DataBuffer messageBuffer)
		{
			return messageBuffer.Bytes[messageBuffer.Length - 1]
				!= ProtocolUtilities.CalcLrc(messageBuffer.Bytes, 1, messageBuffer.Length - 1);
		}
	}
}
