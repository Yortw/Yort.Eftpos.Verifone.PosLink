using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	internal class ProtocolUtilities
	{
		public static byte CalcLrc(byte[] data, int offset, int length)
		{
			byte retVal = 0;

			for (int i = 0; i < length; i++)
				retVal ^= data[offset + i];

			return retVal;
		}

		public static byte CalcLrc(System.IO.Stream stream, int offset)
		{
			byte retVal = 0;

			stream.Seek(offset, System.IO.SeekOrigin.Begin);

			int value = 0;
			while ((value = stream.ReadByte()) != -1)
			{ 
				retVal ^= (byte)value;
			}

			return retVal;
		}

		public static string EncodeSpecialCharacters(string value)
		{
			if (value.IndexOfAny(ProtocolConstants.ControlChars) < 0) return value;

			var sb = new StringBuilder(Convert.ToInt32(Math.Ceiling(value.Length * 1.5)));

			for (var cnt = 0; cnt < value.Length; cnt++)
			{
				var c = value[cnt];
				if (c == ProtocolConstants.ControlChar_Comma)
				{
					sb.Append(ProtocolConstants.ControlChar_FieldSeparator);
					continue;
				}
				else if (IsControlChar(c))
				{
					sb.Append(ProtocolConstants.ControlChar_Dle);
				}
				sb.Append(c);
			}

			return sb.ToString();
		}

		public static string DecodeSpecialCharacters(string value)
		{
			if (value.IndexOfAny(ProtocolConstants.ControlChars) < 0) return value;

			var sb = new StringBuilder(Convert.ToInt32(Math.Ceiling(value.Length * 1.5)));

			for (var cnt = 0; cnt < value.Length; cnt++)
			{
				var c = value[cnt];
				if (c == ProtocolConstants.ControlChar_FieldSeparator)
				{
					sb.Append(ProtocolConstants.ControlChar_Comma);
					continue;
				}
				else if (c == ProtocolConstants.ControlChar_Dle)
				{
					continue;
				}
				sb.Append(c);
			}

			return sb.ToString();
		}


		[System.Runtime.CompilerServices.MethodImpl(methodImplOptions: System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static bool IsControlChar(char c)
		{
			return c == ProtocolConstants.ControlChar_Ack
				|| c == ProtocolConstants.ControlChar_Comma
				|| c == ProtocolConstants.ControlChar_Dle
				|| c == ProtocolConstants.ControlChar_Enq
				|| c == ProtocolConstants.ControlChar_Etx
				|| c == ProtocolConstants.ControlChar_FieldSeparator
				|| c == ProtocolConstants.ControlChar_Nack
				|| c == ProtocolConstants.ControlChar_Stx;
		}
	}
}