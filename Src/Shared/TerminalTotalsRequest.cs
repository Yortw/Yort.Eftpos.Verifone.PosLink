using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests totals for the terminal be returned, and optionally reset.
	/// </summary>
	/// <seealso cref="TerminalTotalsResponse"/>
	public sealed class TerminalTotalsRequest : PosLinkTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public TerminalTotalsRequest() : base()
		{
		}

		/// <summary>
		/// Returns "TOL".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_TerminalTotals; } }

		/// <summary>
		/// Sets or returns a boolean indicating whether or not the totals should be reset after being returned.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.YesNoBoolean, MaxLength = 1, Required = true, Sequence = 3)]
		public bool ResetTotals { get; set; }

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