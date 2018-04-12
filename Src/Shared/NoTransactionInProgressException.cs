using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Throw if an <see cref="EftposCancelRequest"/> is made while no transaction is in progress to be cancelled.
	/// </summary>
#if SUPPORTS_SERIALIZATION
	[Serializable]
#endif
	public class NoTransactionInProgressException : Exception
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public NoTransactionInProgressException() : this(ErrorMessages.NoTransactionInProgress) { } 
		/// <summary>
		/// Partial constructor with custom error message.
		/// </summary>
		/// <param name="message">The error message to use. If null the default message will be used.</param>
		public NoTransactionInProgressException(string message) : base(message ?? ErrorMessages.NoTransactionInProgress) { }
		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="message">The error message to use. If null the default message will be used.</param>
		/// <param name="inner">Another exception to be wrapped by this one which is the root exception.</param>
		public NoTransactionInProgressException(string message, Exception inner) : base(message ?? ErrorMessages.NoTransactionInProgress, inner) { }
#if SUPPORTS_SERIALIZATION
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected NoTransactionInProgressException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
	}
}