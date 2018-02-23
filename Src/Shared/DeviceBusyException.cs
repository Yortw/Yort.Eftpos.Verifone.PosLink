using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Thrown when the device report's itself as busy to a new request, or does not ack or respond within the expected period.
	/// </summary>
	[Serializable]
	public class DeviceBusyException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public DeviceBusyException() : this(ErrorMessages.TerminalBusy) { } 
		/// <summary>
		/// Constructor with custom errror message.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		public DeviceBusyException(string message) : base(message) { }
		/// <summary>
		/// Constructor with custom errror message and wrapped exception.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		/// <param name="inner">The exception to wrap.</param>
		public DeviceBusyException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected DeviceBusyException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
