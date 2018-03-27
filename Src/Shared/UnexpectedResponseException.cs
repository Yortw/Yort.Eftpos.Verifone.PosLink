using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
#if SUPPORTS_SERIALIZATION
		[Serializable]
#endif
	public sealed class UnexpectedResponseException : PosLinkProtocolException
	{
		private readonly PosLinkResponseBase _Response;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public UnexpectedResponseException() : this(ErrorMessages.UnexpectedResponse)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="errorMessage">The error message to associated with this exception.</param>
		public UnexpectedResponseException(string errorMessage) : base(errorMessage)
		{
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="errorMessage">The error message to associated with this exception.</param>
		/// <param name="innerException">Another exception that is wrapped by this one.</param>
		public UnexpectedResponseException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
		{
		}

		/// <summary>
		/// Recommended constructor.
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

#if SUPPORTS_SERIALIZATION

		/// <summary>
		/// Assists with serialisation of this exception.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		private UnexpectedResponseException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
	}
}