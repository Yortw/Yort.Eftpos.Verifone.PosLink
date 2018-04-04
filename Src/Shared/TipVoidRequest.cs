using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request to void a tip pre-auth transaction. In the case of a Tip Pre-Auth not receiving a valid Signature, or if for some reason there is a need to cancel the original transaction, the Tip Void facility will be required. This will result in the merchant not being paid for the original Tip Pre-Auth. 
	/// </summary>
	/// <seealso cref="PosLinkTransactionRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="TipVoidResponse"/>
	/// <seealso cref="TipPreauthRequest"/>
	/// <seealso cref="TipPreauthManualPanRequest"/>
	public class TipVoidRequest : PosLinkTransactionRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TipVoidRequest() : base()
		{
		}

		/// <summary>
		/// Returns "TAR".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_TipVoid;	}	}

		/// <summary>
		/// Sets or returns the total transaction amount including tip if added.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public decimal TotalAmount { get; set; }

		/// <summary>
		/// Sets or returns the STAN returned from the tip pre-auth.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 999, Required = false, Sequence = 4)]
		public string Stan { get; set; }

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
		/// <item><see cref="TotalAmount"/> is greater than zero.</item>
		/// <item><see cref="Stan"/> is not null or empty.</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			TotalAmount.GuardZeroOrNegative(nameof(TotalAmount));
			Stan.GuardNullOrWhiteSpace(nameof(Stan));
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}

	}
}