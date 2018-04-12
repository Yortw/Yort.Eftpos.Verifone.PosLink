using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request for payment with optional cash out.
	/// </summary>
	/// <seealso cref="PosLinkTransactionRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	public class PurchaseRequest : PosLinkTransactionRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PurchaseRequest() : base()
		{
		}

		/// <summary>
		/// Returns "PUR".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_Purchase;	}	}

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
		/// <remarks>
		/// <para>Limited to 10 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 5)]
		public string Id { get; set; }

		/// <summary>
		/// Validates the properties of this message are valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Performs the following validations in addition to those provided by base classes;
		/// <list type="bullet">
		/// <item><see cref="PurchaseAmount"/> is greater than zero</item>
		/// <item><see cref="CashAmount"/> is not negative</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			PurchaseAmount.GuardZeroOrNegative(nameof(PurchaseAmount));
			CashAmount.GuardNegative(nameof(CashAmount));
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

		/// <summary>
		/// Returns the value of the <see cref="PurchaseAmount"/> property.
		/// </summary>
		/// <returns>Returns the value of the <see cref="PurchaseAmount"/> property</returns>
		public override decimal GetManualResponseTransactionAmount()
		{
			return PurchaseAmount;
		}

	}
}