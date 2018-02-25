using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request from the terminal for the customer to sign a receipt and the operator to approve or reject the signature.
	/// </summary>
	/// <remarks>
	/// <para>Typically client code does not deal with this message type itself, rather it subscribes to the <see cref="PinpadClient.QueryOperator"/> event.</para>
	/// </remarks>
	/// <seealso cref="SignatureRequest"/>
	/// <seealso cref="PinpadClient.QueryOperator"/>
	public class SignatureResponse : PosLinkRequestBase
	{
		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		public SignatureResponse() 
		{
		}

		/// <summary>
		/// Returns the value "SIG".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_Sig; } }

		/// <summary>
		/// The text of the prompt to be displayed to the operator.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 3)]
		public string Response { get; set; }

		/// <summary>
		/// Sets or returns the number of the merchant account to conduct the transaction under.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 2)]
		public int Merchant { get; set; } = GlobalSettings.MerchantId;

		/// <summary>
		/// Validates that <see cref="Response"/> is either "YES" or "NO".
		/// </summary>
		public override void Validate()
		{
			if (this.Response != ProtocolConstants.Response_Yes && this.Response != ProtocolConstants.Response_No) throw new ArgumentException(ErrorMessages.ResponseMustBeYesOrNo);

			base.Validate();
		}
	}
}