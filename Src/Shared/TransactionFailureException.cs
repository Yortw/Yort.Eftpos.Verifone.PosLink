using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a critical failure to determine the status of a transaction.
	/// </summary>
	/// <remarks>
	/// <para>Clients must handle this exception and ask the operator to check the pin pad then manually advise whether the transaction was successful or not.</para>
	/// </remarks>
	[Serializable]
	public class TransactionFailureException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TransactionFailureException() : this(ErrorMessages.TransactionFailure) { }
		/// <summary>
		/// Constructor with custom error message.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		public TransactionFailureException(string message) : base(message) { }
		/// <summary>
		/// Constructor with custom error message and wrapped exception.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		/// <param name="inner">The exception to wrap.</param>
		public TransactionFailureException(string message, Exception inner) : base(message, inner) { }
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected TransactionFailureException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}