using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Thrown when the pin pad sends an explicit negative acknowledgement to a request.
	/// </summary>
	[Serializable]
	public class PosLinkNackException : PosLinkProtocolException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public PosLinkNackException() : this(ErrorMessages.NackExceptionMessage) { }
		/// <summary>
		/// Constructor with custom errror message.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		public PosLinkNackException(string message) : base(message) { }
		/// <summary>
		/// Constructor with custom errror message and wrapped exception.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		/// <param name="inner">The exception to wrap.</param>
		public PosLinkNackException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected PosLinkNackException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
