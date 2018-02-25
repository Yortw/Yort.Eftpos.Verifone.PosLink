using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents an error that occurred at the protocol level.
	/// </summary>
#if SUPPORTS_SERIALIZATION
	[Serializable]
#endif
	public class PosLinkProtocolException : Exception
	{
		/// <summary>
		/// Default constructor. Not recommended, provides no error message.
		/// </summary>
		public PosLinkProtocolException() { }
		/// <summary>
		/// Constructor with custom error message.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		public PosLinkProtocolException(string message) : base(message) { }
		/// <summary>
		/// Constructor with custom error message and a pin pad response code.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		/// <param name="responseCode">The response code returned by the pin pad, if any.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public PosLinkProtocolException(string message, string responseCode) : base(message)
		{
			if (!String.IsNullOrWhiteSpace(responseCode))
				this.Data["ResponseCode"] = responseCode;
		}
		/// <summary>
		/// Constructor with custom error message and wrapped exception.
		/// </summary>
		/// <param name="message">The error message to associate with this exception.</param>
		/// <param name="inner">The exception to wrap.</param>
		public PosLinkProtocolException(string message, Exception inner) : base(message, inner) { }
#if SUPPORTS_SERIALIZATION
		/// <summary>
		/// Deserialisation constructor.
		/// </summary>
		/// <param name="info">The serialization information used to deserialise the exception.</param>
		/// <param name="context">The streaming context used to deserialise the exception.</param>
		protected PosLinkProtocolException(
		System.Runtime.Serialization.SerializationInfo info,
		System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
		/// <summary>
		/// The response code returned by the device, if any, otherwise null.
		/// </summary>
		public string ResponseCode
		{
			get
			{
				if (Data.Contains("ResponseCode"))
					return (string)Data["ResponseCode"];

				return null;
				}
		}
	}
}