using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests a logon to the payment gateway for the specified merchant account.
	/// </summary>
	/// <seealso cref="LogonResponseMessage"/>
	public sealed class LogonRequestMessage : PosLinkTransactionRequestMessageBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LogonRequestMessage() : base()
		{
		}

		/// <summary>
		/// Returns "LOG" indicating this is a logon request.
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_Logon; } }

		/// <summary>
		/// Unused. Ignored by the terminal but required to keep other fields in order. Will be sent as an empty text field.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 3)]
		public string Id { get { return String.Empty; } }
	}
}