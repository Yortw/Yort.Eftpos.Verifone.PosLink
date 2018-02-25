using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request from the terminal for the user be asked a question.
	/// </summary>
	/// <seealso cref="AskResponse"/> 
	public class AskRequest : PosLinkResponseBase
	{
		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <remarks>
		/// <para>Typically client code does not deal with this message type itself, rather it subscribes to the <see cref="PinpadClient.QueryOperator"/> event.</para>
		/// </remarks>
		/// <seealso cref="PinpadClient.QueryOperator"/>
		public AskRequest(IList<string> fieldValues) : base(fieldValues)
		{
		}

		/// <summary>
		/// Returns the value "ASK".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Ask; } }

		/// <summary>
		/// Sets or returns the number of the merchant account to conduct the transaction under.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 2)]
		public int Merchant { get; set; } = GlobalSettings.MerchantId;

		/// <summary>
		/// The text of the prompt to be displayed to the operator.
		/// </summary>
		public string Prompt { get { return Fields[3]; } }

	}
}