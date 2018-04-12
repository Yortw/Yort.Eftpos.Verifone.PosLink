using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests a totals for the terminal be returned, and optionally reset.
	/// </summary>
	/// <seealso cref="SettlementCutoverResponse"/>
	public sealed class SettlementCutoverRequest : PosLinkTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SettlementCutoverRequest() : base()
		{
		}

		/// <summary>
		/// Returns "SET".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_SettlementCutover; } }

		/// <summary>
		/// Not used. Ignored by terminal, but required by protocol. Leave empty.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = false, Sequence = 4)]
		public string Id { get; set; }

		/// <summary>
		/// Returns zero.
		/// </summary>
		/// <returns>Returns zero.</returns>
		public override decimal GetManualResponseTransactionAmount()
		{
			return 0;
		}

	}
}