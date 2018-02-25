using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Contains the response to a <see cref="QueryCardRequest"/>.
	/// </summary>
	/// <seealso cref="QueryCardRequest"/>
	public sealed class QueryCardResponse : PosLinkResponseBase
	{

		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="PollRequest"/>
		public QueryCardResponse(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "QCD".
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_QueryCard; } }

		/// <summary>
		/// The response code of the transaction.
		/// </summary>
		/// <seealso cref="ResponseCodes"/>
		public string Response { get { return Fields[3]; } }

		/// <summary>
		/// Any text to be displayed to the operator.
		/// </summary>
		public string Display { get { return Fields[4]; } }

		/// <summary>
		/// The ASCII representation of track 2 details.
		/// </summary>
		public string CardTrack2 { get { return FieldValueOrNull(5); } }

		/// <summary>
		/// The ASCII representation of track 1 details.
		/// </summary>
		public string CardTrack1 { get { return FieldValueOrNull(6); } }

	}
}