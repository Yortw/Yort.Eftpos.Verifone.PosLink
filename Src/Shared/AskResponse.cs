using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a request from the terminal for the user be asked a question.
	/// </summary>
	/// <remarks>
	/// <para>Typically client code does not deal with this message type itself, rather it subscribes to the <see cref="PinpadClient.QueryOperator"/> event.</para>
	/// </remarks>
	/// <seealso cref="AskRequest"/>
	/// <seealso cref="PinpadClient.QueryOperator"/>
	public class AskResponse : PosLinkRequestBase
	{
		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		public AskResponse() 
		{
		}

		/// <summary>
		/// Returns the value "ASK".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_Ask; } }

		/// <summary>
		/// The text of the prompt to be displayed to the operator.
		/// </summary>
		public string Response { get; set; }

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