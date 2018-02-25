using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests the terminal prompt for a card swipe and return the card track data.
	/// </summary>
	/// <remarks>
	/// <para>Generic “Card Swipe” functionality is supported by the terminal.  The POS will be provided with standard ISO 7813 compliant formatted data from the users’ card swipe. This functionality is restricted to proprietary 3rd party issued cards.</para>
	/// <para>To assist in PA-DSS compliance (PCI security requirement), any merchant requiring this functionality to be enabled must provide the EFTPOS reseller with a positive BIN table. This should consist of each card prefix (first 6 or more digits). The EFTPOS reseller will create an encrypted table to be loaded on the terminal.</para>
	/// </remarks>
	/// <seealso cref="QueryCardResponse"/>
	public sealed class QueryCardRequest : PosLinkRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public QueryCardRequest() : base()
		{
		}

		/// <summary>
		/// Returns "QCD".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_QueryCard; } }

		/// <summary>
		/// Sets or returns a boolean indicating whether or not the totals should be reset after being returned.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.YesNoBoolean, MaxLength = 1, Required = true, Sequence = 3)]
		public bool ResetTotals { get; set; }

		/// <summary>
		/// Not used. Ignored by terminal, but required by protocol. Leave empty.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = true, Sequence = 4)]
		public string Id { get; set; }

	}
}