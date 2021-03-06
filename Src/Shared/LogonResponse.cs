using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Contains the pin pad response to a <see cref="LogonRequest"/>.
	/// </summary>
	/// <seealso cref="LogonRequest"/>
	public sealed class LogonResponse : PosLinkResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="LogonRequest"/>
		public LogonResponse(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "LOG".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Logon; } }

		/// <summary>
		/// Returns the response code for this request.
		/// </summary>
		/// <see cref="ResponseCodes"/>
		public string Response { get { return Fields[3]; } }

		/// <summary>
		/// Returns any text to be displayed to the user.
		/// </summary>
		public string Display { get { return Fields[4]; } }

		/// <summary>
		/// Returns the receipt data, if any, to be printed.
		/// </summary>
		public string ReceiptData { get { return FieldValueOrNull(5); } }

	}
}