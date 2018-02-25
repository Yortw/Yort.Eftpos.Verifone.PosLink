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
	/// A class that reads raw data from a stream, parse and decodes the values then deserialises the result into a type derived from <see cref="PosLinkResponseBase"/>.
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
								if (dataBuffer.Bytes[cnt] == ProtocolConstants.ControlByte_Ack)
								{
									GlobalSettings.Logger.LogRx(LogMessages.ReceivedAck, dataBuffer.Bytes, 1);
									return;
								}
								if (dataBuffer.Bytes[cnt] == ProtocolConstants.ControlByte_Nack)
								{
									GlobalSettings.Logger.LogRx(LogMessages.ReceivedNack, dataBuffer.Bytes, 1);
									throw new PosLinkNackException();
								}
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
				GlobalSettings.Logger.LogWarn(LogMessages.NoResponseFromDevice);
				throw new DeviceBusyException();
			}
		}

		/// <summary>
		/// Reads from the stream until a message of type {TResponseMessage} is received.
		/// </summary>
		/// <param name="inStream">The stream to read from.</param>
		/// <param name="outStream">The stream to write acknowledgements to.</param>
		/// <returns>A task whose result is an object of the requested {TResponseMessage} type.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="inStream"/> or <paramref name="outStream"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if <paramref name="inStream"/> is not readable or <paramref name="outStream"/> is not writeable.</exception>
		public async Task<PosLinkResponseBase> ReadMessageAsync(System.IO.Stream inStream, System.IO.Stream outStream) 
		{
			inStream.GuardNull(nameof(inStream));
			if (!inStream.CanRead) throw new ArgumentException(ErrorMessages.StreamMustBeReadable, nameof(inStream));
			outStream.GuardNull(nameof(outStream));
			if (!outStream.CanWrite) throw new ArgumentException(ErrorMessages.StreamMustBeWriteable, nameof(outStream));

			Task sendAckTask = null;
			PosLinkResponseBase message = null;
			while (message == null)
			{
				using (var messageBuffer = await ReadMessageBytes(inStream).ConfigureAwait(false))
				{
					try
					{
						GlobalSettings.Logger.LogRx(LogMessages.ReceivedPacket, messageBuffer.Bytes, messageBuffer.Length);

						var fieldValues = ValidateMessageAndParseFields(messageBuffer);

						sendAckTask = SendAckAsync(outStream);
						message = _MessageFactory.CreateMessage(fieldValues);

						await sendAckTask.ConfigureAwait(false);
						
						if (message == null)
							GlobalSettings.Logger.LogWarn(String.Format(LogMessages.UnknownMessageTypeReceived, fieldValues[1]));
					}
					catch (PosLinkProtocolException ex)
					{
						await SendNackAsync(outStream).ConfigureAwait(false);

						GlobalSettings.Logger.LogError(LogMessages.ErrorReceivingMessage, ex);
					}
				}
			}

			return message;
		}

		private async Task SendAckAsync(Stream outStream)
		{
			if (GlobalSettings.Logger.LogCommunicationPackets)
			{
				using (var stream = GlobalSettings.BufferManager.GetStream())
				{
					stream.WriteByte(ProtocolConstants.ControlByte_Ack);
					stream.Seek(0, SeekOrigin.Begin);
					GlobalSettings.Logger.LogTx("Sending ACK", stream);
				}
			}

			outStream.WriteByte(ProtocolConstants.ControlByte_Ack);
			await outStream.FlushAsync().ConfigureAwait(false);
		}

		private async Task SendNackAsync(Stream outStream)
		{
			outStream.WriteByte(ProtocolConstants.ControlByte_Nack);
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
						if (retVal?.Length == 1)
						{
							if (retVal.Bytes[0] == ProtocolConstants.ControlByte_Ack)
							{
								GlobalSettings.Logger.LogRx(LogMessages.ReceivedAck, retVal.Bytes, 1);
								retVal = null;
								continue;
							}
							else if (retVal.Bytes[0] == ProtocolConstants.ControlByte_Nack)
							{
								GlobalSettings.Logger.LogRx(LogMessages.ReceivedNack, retVal.Bytes, 1);
								throw new PosLinkNackException();
							}
						}

						if (retVal?.Length < ProtocolConstants.ValidMesage_MinBytes)
						{
							GlobalSettings.Logger.LogRx(LogMessages.ReceivedInvalidMessage, retVal.Bytes, retVal.Length);
							throw new PosLinkProtocolException(ErrorMessages.InvalidProtocolMessage);
						}
						cancelTokenSource.Token.ThrowIfCancellationRequested();
					}
					return retVal;
				}
				catch (OperationCanceledException oce)
				{
					throw new DeviceBusyException(ErrorMessages.TerminalBusy, oce);
				}
			}
		}

		private async Task<DataBuffer> ReadData(System.IO.Stream inStream, int maxBytesToRead, System.Threading.CancellationToken cancelToken)
		{
			var messageBuffer = GlobalSettings.BufferManager.GetBuffer();
			try
			{
				var bytesActuallyRead = await inStream.ReadAsync(messageBuffer.Bytes, 0, maxBytesToRead, cancelToken).ConfigureAwait(false);
				if (bytesActuallyRead == 0) return null;

				return new DataBuffer(messageBuffer.Bytes, bytesActuallyRead, messageBuffer);
			}
			catch
			{
				messageBuffer?.Dispose();
				throw;
			}
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
