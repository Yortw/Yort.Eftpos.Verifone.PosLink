using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request to add a tip value to a prior transaction created using <see cref="TipPreauthRequest"/> or <see cref="TipPreauthManualPanRequest"/>.
	/// </summary>
	/// <seealso cref="PosLinkTransactionRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="TipAddResponse"/>
	/// <seealso cref="TipPreauthRequest"/>
	/// <seealso cref="TipPreauthManualPanRequest"/>
	public class TipAddRequest : PosLinkTransactionRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TipAddRequest() : base()
		{
		}

		/// <summary>
		/// Returns "TAR".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_TipAdd;	}	}

		/// <summary>
		/// Sets or returns the original purchase amount from the tip pre-auth.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public decimal OriginalPurchaseAmount { get; set; }

		/// <summary>
		/// Sets or returns the amount of the tip.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 4)]
		public decimal TipAmount { get; set; }

		/// <summary>
		/// Sets or returns the STAN returned from the tip pre-auth.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 999, Required = false, Sequence = 5)]
		public string Stan { get; set; }

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
		/// <item><see cref="OriginalPurchaseAmount"/> is greater than zero.</item>
		/// <item><see cref="TipAmount"/> is greater than zero.</item>
		/// <item><see cref="Stan"/> is not null or empty.</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			OriginalPurchaseAmount.GuardZeroOrNegative(nameof(OriginalPurchaseAmount));
			TipAmount.GuardZeroOrNegative(nameof(TipAmount));
			Stan.GuardNullOrWhiteSpace(nameof(Stan));
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

	}
}