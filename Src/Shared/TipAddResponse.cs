using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a response to a request to add a tip to a transaction.
	/// </summary>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="TipAddRequest"/>
	public sealed class TipAddResponse : PosLinkResponseBase
	{ 	

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="CashOutRequest"/>
		public TipAddResponse(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "TAR".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_TipAdd; } }

		/// <summary>
		/// The purchase value of the original transaction.
		/// </summary>
		public decimal PurchaseAmount { get { return Convert.ToDecimal(Fields[3], System.Globalization.CultureInfo.InvariantCulture); } }

		/// <summary>
		/// The amount of the added tip.
		/// </summary>
		public decimal TipAmount { get { return Convert.ToDecimal(Fields[4], System.Globalization.CultureInfo.InvariantCulture); } }

		/// <summary>
		/// The response code of the transaction.
		/// </summary>
		/// <seealso cref="ResponseCodes"/>
		public string Response { get { return Fields[5]; } }

		/// <summary>
		/// Any text to be displayed to the user.
		/// </summary>
		public string Display { get { return Fields[6]; } }

	}
}