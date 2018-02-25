using Ladon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// A class that encodes friendly message objects into raw protocol bytes and writes them to a stream.
	/// </summary>
	public class MessageWriter
	{

		private static System.Collections.Concurrent.ConcurrentDictionary<Type, IList<PosLinkMessageField>> _MessageFieldCache;
		private static System.Collections.Concurrent.ConcurrentDictionary<Type, object> _DefaultValuesCache;

		private System.Text.ASCIIEncoding _Encoding;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MessageWriter()
		{
			_Encoding = new ASCIIEncoding();
			_MessageFieldCache = _MessageFieldCache ?? new System.Collections.Concurrent.ConcurrentDictionary<Type, IList<PosLinkMessageField>>();
			_DefaultValuesCache = _DefaultValuesCache ?? new System.Collections.Concurrent.ConcurrentDictionary<Type, object>();
		}

		/// <summary>
		/// Writes a message object of type {T} to the <paramref name="outStream"/> as POS Link protocol message.
		/// </summary>
		/// <typeparam name="T">The type of message to write.</typeparam>
		/// <param name="message">The message to be encoded and written.</param>
		/// <param name="outStream">The stream to write the output to.</param>
		/// <returns>A task that can be awaited to ensure completion of the operation and retrieve any exceptions that may have occurred.</returns>
		/// <exception cref="System.ArgumentNullException">Thrown if either <paramref name="message"/> or <paramref name="outStream"/> is null.</exception>
		/// <exception cref="System.ArgumentException">Thrown if either <paramref name="outStream"/> is not writeable.</exception>
		/// <exception cref="System.InvalidOperationException">A message value encoded for the protocol is longer than allowed.</exception>
		public async Task WriteMessageAsync<T>(T message, System.IO.Stream outStream) where T : PosLinkRequestBase
		{
			message.GuardNull(nameof(message));
			message.GuardNull(nameof(outStream));
			if (!outStream.CanWrite) throw new ArgumentException(ErrorMessages.StreamMustBeWriteable, nameof(outStream));
			message.Validate();

			try
			{
				using (var stream = GlobalSettings.BufferManager.GetStream())
				{
					stream.WriteByte(ProtocolConstants.ControlByte_Stx);

					WriteMessageBody(message, stream);

					stream.WriteByte(ProtocolConstants.ControlByte_Etx);

					stream.WriteByte(ProtocolUtilities.CalcLrc(stream, 1));

					stream.Seek(0, SeekOrigin.Begin);

					if (GlobalSettings.Logger.LogCommunicationPackets)
					{
						GlobalSettings.Logger.LogTx(LogMessages.SendingPacket, stream);
						stream.Seek(0, SeekOrigin.Begin);
					}

					await stream.CopyToAsync(outStream).ConfigureAwait(false);
					await outStream.FlushAsync().ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				GlobalSettings.Logger.LogError(LogMessages.ErrorSendingMessage, ex);
				throw;
			}
		}

		private void WriteMessageBody<T>(T message, System.IO.Stream stream)
		{
			var fields = GetFieldsForMessageType(message);

			var doneOne = false;
			foreach (var field in fields)
			{
				if (doneOne)
					stream.WriteByte(ProtocolConstants.ControlByte_Comma);
				else
					doneOne = true;

				WriteField(field, message, stream);
			}
		}

		private IList<PosLinkMessageField> GetFieldsForMessageType<T>(T message)
		{
			var messageType = message.GetType();

			if (_MessageFieldCache.TryGetValue(messageType, out var fields))
				return fields;

			var newFields =
			(
				from p
				in messageType.GetProperties()
				select CreateFieldFromProperty(p)
			);

			fields = new List<PosLinkMessageField>
			(
				from f
				in newFields
				where f != null
				orderby f.Sequence
				select f
			);

			_MessageFieldCache.TryAdd(messageType, fields);

			return fields;
		}

		private PosLinkMessageField CreateFieldFromProperty(PropertyInfo property)
		{
			var attribute = property.GetCustomAttribute<PosLinkMessageFieldAttribute>();
			if (attribute == null) return null;

			return new PosLinkMessageField()
			{
				Property = property,
				Format = attribute.Format,
				MaxLength = attribute.MaxLength,
				Sequence = attribute.Sequence,
				IsRequired = attribute.Required
			};
		}

		private void WriteField<T>(PosLinkMessageField field, T message, Stream stream)
		{
			var value = FormatValue(field.GetValue(message), field);
			var data = new byte[512];
			var byteLength = _Encoding.GetBytes(value, 0, value.Length, data, 0);
			stream.Write(data, 0, byteLength);
		}

		private static string FormatValue(object value, PosLinkMessageField field)
		{
			if (field.IsRequired)
			{
				if (value is string valueString)
					valueString.GuardNullOrWhiteSpace(nameof(valueString));

				value.GuardNull(nameof(value));

				var type = value.GetType();
				if (type.IsValueType)
				{
					if (!_DefaultValuesCache.TryGetValue(type, out var defaultValue))
					{
						defaultValue = Activator.CreateInstance(type);
						_DefaultValuesCache.TryAdd(type, defaultValue);
					}		
					if (value == defaultValue) throw new ArgumentException(ErrorMessages.ValueIsRequired, nameof(value));
				}
			}

			var retVal = String.Empty;
			switch (field.Format)
			{
				case PosLinkMessageFieldFormat.Text:
					if (value != null)
						retVal = ProtocolUtilities.EncodeSpecialCharacters(value.ToString());
					break;

				case PosLinkMessageFieldFormat.YesNoBoolean:
					retVal = ((bool)value) ? "Y" : "N";
					break;

				case PosLinkMessageFieldFormat.ZeroPaddedNumber:
					retVal = value.ToString().PadLeft(field.MaxLength, '0');
					break;

				case PosLinkMessageFieldFormat.DateDdMmYyyy:
					if (value == null)
						retVal = String.Empty;
					else
						retVal = ((DateTime)value).ToString("ddMMyyyy");

					break;
				default:
					throw new InvalidOperationException(ErrorMessages.UnknownMessageFieldFormat);
			}

			if (field.MaxLength > 0 && retVal.Length > field.MaxLength)
				throw new ArgumentException(String.Format(ErrorMessages.ValueIsTooLong, field.Property.Name));

			return retVal;
		}

	}
}