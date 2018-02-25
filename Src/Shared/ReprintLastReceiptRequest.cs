using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests the last EFTPOS receipt be printed (or returns so it can be printed by the client).
	/// </summary>
	/// <seealso cref="ReprintLastReceiptRequest"/>
	public sealed class ReprintLastReceiptRequest : PosLinkTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public ReprintLastReceiptRequest() : base()
		{
		}

		/// <summary>
		/// Returns "REP".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_ReprintLastReceipt; } }

		/// <summary>
		/// Not used. Ignored by terminal, but required by protocol. Leave empty.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 3)]
		public string Id { get; set; }

	}
}