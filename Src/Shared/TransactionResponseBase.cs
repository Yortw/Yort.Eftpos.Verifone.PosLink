using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a response to a request for a payment and/or cash out transaction.
	/// </summary>
	/// <remarks>
	/// <para>Can also be used for cash out without a purchase amount by leaving <see cref="PurchaseAmount"/> as zero and providing a value only for <see cref="CashAmount"/>.</para>
	/// </remarks>
	/// <seealso cref="PurchaseRequest"/>
	public abstract class TransactionResponseBase : PosLinkResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="PollRequest"/>
		protected TransactionResponseBase(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "PUR".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Purchase; } }

		/// <summary>
		/// The purchase value originally requested.
		/// </summary>
		public decimal PurchaseAmount { get { return Convert.ToDecimal(Fields[3], System.Globalization.CultureInfo.InvariantCulture); } }

		/// <summary>
		/// The amount of cash out originally requested.
		/// </summary>
		public decimal CashAmount { get { return Convert.ToDecimal(Fields[4], System.Globalization.CultureInfo.InvariantCulture); } }

		/// <summary>
		/// The response code of the transaction.
		/// </summary>
		/// <seealso cref="ResponseCodes"/>
		public string Response { get { return Fields[5]; } }

		/// <summary>
		/// Any text to be displayed to the user.
		/// </summary>
		public string Display { get { return Fields[6]; } }

		/// <summary>
		/// The reference for this transaction provided by the bank/payment gateway.
		/// </summary>
		public string BankReference { get { return Fields[7]; } }

		/// <summary>
		/// The System Trace Audit Number returned by the pin pad, unique per device transaction.
		/// </summary>
		public string Stan { get { return Fields[8]; } }

		/// <summary>
		/// The truncated card number returned by the device.
		/// </summary>
		public string TruncatedPan { get { return Fields[9]; } }

		/// <summary>
		/// The type of account used with the card.
		/// </summary>
		public string Account { get { return Fields[10]; } }

		/// <summary>
		/// The merchant receipt text, if any, that must be printed.
		/// </summary>
		public string MerchantReceipt { get { return FieldValueOrNull(11); } }
		/// <summary>
		/// The customer receipt text, if any, that may be optionally printed.
		/// </summary>
		public string CustomerReceipt { get { return FieldValueOrNull(12); } }


	}
}