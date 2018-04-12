using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request for cash out with no purchase/payment value.
	/// </summary>
	/// <seealso cref="PosLinkTransactionOptionsRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="CashOutResponse"/>
	public class CashOutRequest : PosLinkFinancialTransactionRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CashOutRequest() : base()
		{
		}

		/// <summary>
		/// Returns "CSH".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_CashOnly;	}	}

		/// <summary>
		/// Sets or returns the amount of the cash out.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public override decimal Amount { get; set; }

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