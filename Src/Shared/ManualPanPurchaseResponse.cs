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
	/// <para>Can also be used for cash out without a purchase amount by leaving <see cref="TransactionResponseBase.PurchaseAmount"/> as zero and providing a value only for <see cref="TransactionResponseBase.CashAmount"/>.</para>
	/// </remarks>
	/// <seealso cref="ManualPanPurchaseRequest"/>
	public sealed class ManualPanPurchaseResponse : TransactionResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="PollRequest"/>
		public ManualPanPurchaseResponse(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "MAN".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_ManualPanPurchase; } }
		
	}
}