using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a response to a request for a tip batch upload.
	/// </summary>
	/// <seealso cref="PosLinkResponseBase"/>
	/// <seealso cref="TipBatchUploadRequest"/>
	public class TipBatchUploadResponse : PosLinkResponseBase
	{
		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="ChequeAuthorisationRequest"/>
		public TipBatchUploadResponse(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "TBU".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_TipBatchUpload; } }

		/// <summary>
		/// The response code of the transaction.
		/// </summary>
		/// <seealso cref="ResponseCodes"/>
		public string Response { get { return Fields[3]; } }

		/// <summary>
		/// Any text to be displayed to the user.
		/// </summary>
		public string Display { get { return Fields[4]; } }

		/// <summary>
		/// The merchant receipt text, if any, that must be printed.
		/// </summary>
		public string MerchantReceipt { get { return FieldValueOrNull(5); } }
		/// <summary>
		/// The customer receipt text, if any, that may be optionally printed.
		/// </summary>
		public string CustomerReceipt { get { return FieldValueOrNull(6); } }

	}
}