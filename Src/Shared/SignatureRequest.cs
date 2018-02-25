using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request from the terminal for the user be asked a question.
	/// </summary>
	public class SignatureRequest : PosLinkResponseBase
	{
		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <remarks>
		/// <para>Typically client code does not deal with this message type itself, rather it subscribes to the <see cref="PinpadClient.QueryOperator"/> event.</para>
		/// </remarks>
		/// <seealso cref="PinpadClient.QueryOperator"/>
		/// <seealso cref="SignatureResponse"/> 
		public SignatureRequest(IList<string> fieldValues) : base(fieldValues)
		{
		}

		/// <summary>
		/// Returns the value "SIG".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Sig; } }

		/// <summary>
		/// Sets or returns the number of the merchant account to conduct the transaction under.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 2)]
		public int Merchant { get; set; } = GlobalSettings.MerchantId;

		/// <summary>
		/// The text of the prompt to be displayed to the operator.
		/// </summary>
		public string Prompt { get { return Fields[3]; } }

		/// <summary>
		/// The merchant receipt text. Preformatted receipt data. If the POS previously requested receipt data, the signature capture receipt.
		/// </summary>
		public string ReceiptText { get { return Fields[3]; } }

	}
}