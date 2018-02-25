using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests totals for the terminal be returned, and optionally reset.
	/// </summary>
	/// <seealso cref="SettlementEnquiryResponse"/>
	public sealed class SettlementEnquiryRequest : PosLinkTransactionRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public SettlementEnquiryRequest() : base()
		{
		}

		/// <summary>
		/// Returns "ENQ".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_SettlementEnquiry; } }

		/// <summary>
		/// Requested settlement date (within 7 days). Optional.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.DateDdMmYyyy, MaxLength = 1, Required = true, Sequence = 3)]
		public DateTime? SettlementDate { get; set; }

		/// <summary>
		/// Not used. Ignored by terminal, but required by protocol. Leave empty.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = true, Sequence = 4)]
		public string Id { get; set; }

	}
}