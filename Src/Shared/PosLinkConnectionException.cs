using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Thrown when a connection cannot be established with the pinpad device.
	/// </summary>
	/// <remarks>
	/// <para>This exception is only thrown if all connection attempts for a request fail, if any connection attempt succeeds this exception will not be thrown even if a subsequent connection failure happens (a different exception will be thrown). 
	/// This means clients can be sure that if this exception type is caught no attempt was made to send the request to the pin pad and no recovery action for the request is required.</para>
	/// </remarks>
#if SUPPORTS_SERIALIZATION
	[Serializable]
#endif
	public class PosLinkConnectionException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PosLinkConnectionException() : this(ErrorMessages.ConnectionFailed) { }
		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="message">The error message to associate with this exception. If null the default error message is used.</param>
		public PosLinkConnectionException(string message) : base(message ?? ErrorMessages.ConnectionFailed) { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="message">The error message to associate with this exception. If null the default error message is used.</param>
		/// <param name="inner"></param>
		public PosLinkConnectionException(string message, Exception inner) : base(message ?? ErrorMessages.ConnectionFailed, inner) { }
#if SUPPORTS_SERIALIZATION
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected PosLinkConnectionException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
	}
}
