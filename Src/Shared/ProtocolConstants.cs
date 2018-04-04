using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	internal static class ProtocolConstants
	{

		public const byte ControlByte_Ack = 0x06;
		public const byte ControlByte_Comma = 0x2C;
		public const byte ControlByte_Dle = 0x10;
		public const byte ControlByte_Enq = 0x05;
		public const byte ControlByte_Etx = 3;
		public const byte ControlByte_FieldSeparator = 0x1C;
		public const byte ControlByte_Nack = 0x15;
		public const byte ControlByte_Stx = 2;

		public const char ControlChar_Ack = (char)ControlByte_Ack;
		public const char ControlChar_Comma = (char)ControlByte_Comma;
		public const char ControlChar_Dle = (char)ControlByte_Dle;
		public const char ControlChar_Enq = (char)ControlByte_Enq;
		public const char ControlChar_Etx = (char)ControlByte_Etx;
		public const char ControlChar_FieldSeparator = (char)ControlByte_FieldSeparator;
		public const char ControlChar_Nack = (char)ControlByte_Nack;
		public const char ControlChar_Stx = (char)ControlByte_Stx;

		public static readonly char[] ControlChars = new char[] { ControlChar_Ack, ControlChar_Comma, ControlChar_Dle, ControlChar_Enq, ControlChar_Etx, ControlChar_FieldSeparator, ControlChar_Nack, ControlChar_Stx };

		public const string MessageType_Logon = "LOG";

		public const string MessageType_Ask = "ASK";
		public const string MessageType_Cancel = "CAN";
		public const string MessageType_ChequeAuthorisation = "CHQ";
		public const string MessageType_CashOnly = "CSH";
		public const string MessageType_Display = "DSP";
		public const string MessageType_Error = "ERR";
		public const string MessageType_ManualPanPurchase = "MAN";
		public const string MessageType_ManualPanRefund = "MRF";
		public const string MessageType_Poll = "POL";
		public const string MessageType_Purchase = "PUR";
		public const string MessageType_QueryCard = "QCD";
		public const string MessageType_Refund = "REF";
		public const string MessageType_ReprintLastReceipt = "REP";
		public const string MessageType_SettlementCutover = "SET";
		public const string MessageType_SettlementEnquiry = "ENQ";
		public const string MessageType_Sig = "SIG";
		public const string MessageType_TerminalTotals = "TOL";
		public const string MessageType_TipAdd = "TAR";
		public const string MessageType_TipBatchUpload = "TBU";
		public const string MessageType_TipPreauth = "TPA";
		public const string MessageType_TipPreauthManualPan = "TPM";
		public const string MessageType_TipVoid = "TVD";

		public const int MinMerchantId = 1;
		public const int MaxMerchantId = 8;

		public const string ProtocolVersion_2Dot2 = "2.2";

		public const int Timeout_ReceiveAck_Milliseconds = 3000;
		public const int Timeout_ReadResponse_Milliseconds = 60000;
		public const int Timeout_ClearInputBuffer_Milliseconds = 100;

		public const int ReadDelay_Milliseconds = 100;
		public const int RetryDelay_Milliseconds = 100;

		public const int MaxBufferSize_Read = 5500;

		public const int MaxRetries = 3;

		public const int ValidMesage_MinBytes = 5;

		public const int DefaultPort = 4444;

		public const string Response_Yes = "YES";
		public const string Response_No = "NO";

		public static readonly IReadOnlyList<string> DefaultQueryResponses = (new List<string>() { Response_Yes, Response_No }).AsReadOnly();
	}
}