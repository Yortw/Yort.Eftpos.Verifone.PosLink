using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request to begin a purchase that may subsequently have a tip added.
	/// </summary>
	/// <seealso cref="PosLinkTransactionRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="TipPreauthResponse"/>
	public class TipPreauthRequest : PosLinkTransactionRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TipPreauthRequest() : base()
		{
		}

		/// <summary>
		/// Returns "CSH".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_TipPreauth;	}	}

		/// <summary>
		/// Sets or returns the amount of the purchase.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public decimal PurchaseAmount { get; set; }

		/// <summary>
		/// Optional text to be displayed on terminal at Swipe Card prompt.
		/// </summary>
		/// <remarks>
		/// <para>Limited to 10 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 4)]
		public string Id { get; set; }

		/// <summary>
		/// Validates the properties of this message are valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Performs the following validations in addition to those provided by base classes;
		/// <list type="bullet">
		/// <item><see cref="PurchaseAmount"/> is greater than zero.</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			PurchaseAmount.GuardZeroOrNegative(nameof(PurchaseAmount));
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

	}
}