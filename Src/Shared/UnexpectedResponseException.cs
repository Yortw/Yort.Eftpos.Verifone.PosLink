using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Thrown when an unexpected message is received from the pin pad.
	/// </summary>
	/// <remarks>
	/// <para>This error is thrown when a final response message (i.e not an ASK, DSP or SIG message) is received and the message type does not match the request type. For example if you sent a logon message but a terminal totals response was received instead.
	/// Typically this indicates accidental re-use of a <see cref="PosLinkRequestBase.MerchantReference"/> value.</para>
	/// </remarks>
	public class UnexpectedResponseException : PosLinkProtocolException
	{
		private readonly PosLinkResponseBase _Response;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="response">The response message that was received.</param>
		public UnexpectedResponseException(PosLinkResponseBase response) : base(ErrorMessages.UnexpectedResponse)
		{
			_Response = response;
		}

		/// <summary>
		/// The response message that was received from the pin pad.
		/// </summary>
		public PosLinkResponseBase Response
		{
			get { return _Response; }
		}
	}
}