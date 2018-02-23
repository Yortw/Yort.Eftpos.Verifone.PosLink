using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents an error that occurred at the protocol level.
	/// </summary>
	[Serializable]
	public class PosLinkProtocolException : Exception
	{
		/// <summary>
		/// Default constructor. Not recommended, provides no error message.
		/// </summary>
		public PosLinkProtocolException() { }
		/// <summary>
		/// Constructor with custom errror message.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		public PosLinkProtocolException(string message) : base(message) { }
		/// <summary>
		/// Constructor with custom errror message and wrapped exception.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		/// <param name="inner">The exception to wrap.</param>
		public PosLinkProtocolException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected PosLinkProtocolException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}