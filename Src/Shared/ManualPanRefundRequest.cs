using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request for refund of a prior purchase with manual entry of the refund card details.
	/// </summary>
	/// <seealso cref="ManualPanRefundResponse"/>
	public sealed class ManualPanRefundRequest : PosLinkFinancialTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ManualPanRefundRequest() : base()
		{
		}

		/// <summary>
		/// Returns "REF".
		/// </summary>
		public override string RequestType
		{
			get { return ProtocolConstants.MessageType_ManualPanRefund; }
		}

		/// <summary>
		/// Sets or returns the amount of the refund.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public override decimal Amount { get; set; }

		/// <summary>
		/// Read-only property, this argument is obsolete in v2.2 of the specification but required in the message argument list for backwards compatiblity.
		/// </summary>
		/// <remarks>
		/// <para>Returns an empty string. Due to PCI-DSS compliance you can no longer provide card details from the PC, the pin pad will prompt for card details when it receives a MAN request.</para>
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 4, Required = false, Sequence = 4)]
		public string CardExpiryDate { get { return String.Empty; } }

		/// <summary>
		/// Read-only property, this argument is obsolete in v2.2 of the specification but required in the message argument list for backwards compatiblity.
		/// </summary>
		/// <remarks>
		/// <para>Returns an empty string. Due to PCI-DSS compliance you can no longer provide card details from the PC, the pin pad will prompt for card details when it receives a MAN request.</para>
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 16, Required = false, Sequence = 5)]
		public string CardNumber { get { return String.Empty; } }

		/// <summary>
		/// Optional text to be displayed on terminal at Swipe Card prompt.
		/// </summary>
		/// <remarks>
		/// <para>Limited to 10 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 6)]
		public string Id { get; set; }

		/// <summary>
		/// Validates the properties of this message are valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Performs the following validations in addition to those provided by base classes;
		/// <list type="bullet">
		/// <item><see cref="Amount"/> is greater than zero.</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

	}
}