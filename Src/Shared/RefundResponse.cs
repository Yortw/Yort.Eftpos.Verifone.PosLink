using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a response to a request for a refund transaction.
	/// </summary>
	/// <seealso cref="RefundRequest"/>
	public sealed class RefundResponse : TransactionResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="PollRequest"/>
		public RefundResponse(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "REF".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Refund; } }
		
	}
}