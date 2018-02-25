using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents an error response from the pin pad.
	/// </summary>
	public class ErrorResponse : PosLinkResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="PollRequest"/>
		public ErrorResponse(IList<string> fieldValues) : base(fieldValues)
		{
		}

		/// <summary>
		/// Returns the value "ERR".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Error; } }

		/// <summary>
		/// Returns the response code from the pin pad.
		/// </summary>
		/// <see cref="ResponseCodes"/>
		public string Response { get { return Fields[3]; } }

		/// <summary>
		/// Returns text to be displayed to the operator.
		/// </summary>
		/// <see cref="ResponseCodes"/>
		public string Display { get { return Fields[4]; } }

	}
}