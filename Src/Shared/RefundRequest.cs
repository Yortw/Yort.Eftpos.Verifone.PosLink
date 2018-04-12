using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request for refund of a prior purchase.
	/// </summary>
	/// <seealso cref="RefundResponse"/>
	public sealed class RefundRequest : PosLinkTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public RefundRequest() : base()
		{
		}

		/// <summary>
		/// Returns "REF".
		/// </summary>
		public override string RequestType
		{
			get { return ProtocolConstants.MessageType_Refund; }
		}

		/// <summary>
		/// Sets or returns the amount of the refund.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public decimal RefundAmount { get; set; }

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
		/// <item><see cref="RefundAmount"/> is greater than zero.</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			RefundAmount.GuardZeroOrNegative(nameof(RefundAmount));
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

		/// <summary>
		/// Returns the value of the <see cref="RefundAmount"/> property.
		/// </summary>
		/// <returns>Returns the value of the <see cref="RefundAmount"/> property</returns>
		public override decimal GetManualResponseTransactionAmount()
		{
			return RefundAmount;
		}
	}
}