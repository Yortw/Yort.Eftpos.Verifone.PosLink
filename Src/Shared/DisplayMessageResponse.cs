using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a message from the POS to the client asking that an informational/status message be displayed.
	/// </summary>
	public class DisplayMessageResponse : PosLinkResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="DisplayMessage"/>
		public DisplayMessageResponse(IList<string> fieldValues) : base(fieldValues)
		{
		}

		/// <summary>
		/// Returns the value "DSP".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Display; } }

		/// <summary>
		/// Returns the text to be displayed to the operator.
		/// </summary>
		public string MessageText { get { return Fields[3]; } }
	}
}