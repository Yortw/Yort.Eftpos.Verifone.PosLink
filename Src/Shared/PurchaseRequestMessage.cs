using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request for payment (a purchase), with optional cash out amount.
	/// </summary>
	/// <remarks>
	/// <para>Can also be used for cash out without a purchase amount by leaving <see cref="PurchaseAmount"/> as zero and providing a value only for <see cref="CashAmount"/>.</para>
	/// </remarks>
	/// <seealso cref="PurchaseResponseMessage"/>
	public sealed class PurchaseRequestMessage : PosLinkTransactionRequestMessageBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PurchaseRequestMessage() : base()
		{
		}

		/// <summary>
		/// Sets or returns whether or not the card number will be entered manually.
		/// </summary>
		/// <remarks>
		/// <para>PCI-DSS compliance requires the card details be entered on the pinpad, even when <see cref="ManualPan"/> is true you cannot provide the card number via this library or the underlying protocol.</para>
		/// </remarks>
		public bool ManualPan { get; set; }

		/// <summary>
		/// Returns "PUR", "MAN" or "CSH" depending on the value of other properties specified.
		/// </summary>
		/// <remarks>
		/// <para>If <see cref="ManualPan"/> is true then MAN is returned. If <see cref="PurchaseAmount"/> is zero and <see cref="CashAmount"/> is not zero, CSH is returned. Otherwise PUR is returned.</para>
		/// </remarks>
		public override string RequestType
		{
			get
			{
				if (this.ManualPan)
					return ProtocolConstants.MessageType_ManualPan;
				else if (this.PurchaseAmount == 0 && this.CashAmount != 0)
					return ProtocolConstants.MessageType_CashOnly;

				return ProtocolConstants.MessageType_Purchase;
			}
		}

		/// <summary>
		/// Sets or returns the amount of the purchase.
		/// </summary>
		/// <remarks>
		/// <para>May be left as zero if only cash out is desired. Must not be negative.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public decimal PurchaseAmount { get; set; }

		/// <summary>
		/// Sets or returns the amount of the cash out.
		/// </summary>
		/// <remarks>
		/// <para>May be left as zero if no cash out is desired. Must not be negative.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 4)]
		public decimal CashAmount { get; set; }

		/// <summary>
		/// Optional text to be displayed on terminal at Swipe Card prompt.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 5)]
		public string Id { get; set; }

		/// <summary>
		/// Validates the properties of this message are valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Performs the following validations in addition to those provided by base classes;
		/// <list type="bullet">
		/// <item><see cref="PurchaseAmount"/> is not negative</item>
		/// <item><see cref="CashAmount"/> is not negative</item>
		/// <item>Either <see cref="PurchaseAmount"/> or <see cref="CashAmount"/> has a value greater than zero</item>
		/// <item>If <see cref="ManualPan"/> is true thjen <see cref="CashAmount"/> must be zero</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			PurchaseAmount.GuardNegative(nameof(PurchaseAmount));
			PurchaseAmount.GuardNegative(nameof(CashAmount));
			if (this.PurchaseAmount == 0 && this.CashAmount == 0) throw new ArgumentException(ErrorMessages.PurchaseAmountOrCashAmountMustBeSpecified, nameof(PurchaseAmount));
			if (this.ManualPan && this.CashAmount != 0) throw new ArgumentException(ErrorMessages.CashOutNotAllowedWithManualPan, nameof(CashAmount));

			base.Validate();
		}

	}
}