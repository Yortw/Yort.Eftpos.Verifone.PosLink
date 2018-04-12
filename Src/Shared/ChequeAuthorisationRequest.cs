using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request to authorise a cheque.
	/// </summary>
	/// <seealso cref="PosLinkTransactionRequestBase"/>
	/// <seealso cref="TransactionResponseBase"/>
	/// <seealso cref="ChequeAuthorisationResponse"/>
	public class ChequeAuthorisationRequest : PosLinkTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ChequeAuthorisationRequest() : base()
		{
		}

		/// <summary>
		/// Returns "CHQ".
		/// </summary>
		public override string RequestType => ProtocolConstants.MessageType_ChequeAuthorisation;

		/// <summary>
		/// Sets or returns the amount of the cash out.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.ZeroPaddedNumber, MaxLength = 9, Required = false, Sequence = 3)]
		public decimal Amount { get; set; }

		/// <summary>	
		/// Sets or returns the bank and branch number associated with the cheque.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.TextRightPadded, MaxLength = 8, Required = false, Sequence = 4)]
		public string BankAndBranchNumber { get; set; }

		/// <summary>
		/// Sets or returns the account number associated with the cheque.
		/// </summary>
		/// <remarks>
		/// <para>Maximum length is 10 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.TextRightPadded, MaxLength = 10, Required = false, Sequence = 5)]
		public string AccountNumber { get; set; }

		/// <summary>
		/// Sets or returns the serial number of the cheque. 
		/// </summary>
		/// <remarks>
		/// <para>Maximum length is 6 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 6, Required = false, Sequence = 6)]
		public string SerialNumber { get; set; }

		/// <summary>
		/// Optional text to be displayed on terminal at Swipe Card prompt.
		/// </summary>
		/// <remarks>
		/// <para>Limited to 10 characters.</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 7)]
		public string Id { get; set; }

		/// <summary>
		/// Validates the properties of this message are valid.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Performs the following validations in addition to those provided by base classes;
		/// <list type="bullet">
		/// <item><see cref="Amount"/> is gerater than zero</item>
		/// <item>If <see cref="BankAndBranchNumber"/>.Length is not more than 10 characters and not null or empty.</item>
		/// <item>If <see cref="AccountNumber"/>.Length is not more than 8 characters and not null or empty.</item>
		/// <item>If <see cref="SerialNumber"/>.Length is exactly 6 characters and not null, empty or whitespace.</item>
		/// <item>If <see cref="Id"/>.Length is not more than 10 characters.</item>
		/// </list>
		/// </para>
		/// </remarks>
		public override void Validate()
		{
			Amount.GuardZeroOrNegative(nameof(Amount));
			AccountNumber.GuardNullOrWhiteSpace(nameof(AccountNumber)).Length.GuardRange(nameof(AccountNumber), nameof(AccountNumber.Length), 0, 10);
			SerialNumber.GuardNullOrWhiteSpace(nameof(SerialNumber)).Length.GuardRange(nameof(SerialNumber), nameof(SerialNumber.Length), 6, 6);
			(Id?.Length ?? 0).GuardRange(nameof(Id), nameof(Id.Length), 0, 10);

			base.Validate();
		}
	}
}