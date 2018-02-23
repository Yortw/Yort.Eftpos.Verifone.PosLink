using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Provides a list of known response codes.
	/// </summary>
	public static class ResponseCodes
	{
		/// <summary>
		/// The request was processsed successfully.
		/// </summary>
		public const string Accepted = "00";

		/// <summary>
		/// The request was declined by the device or payment gateway.
		/// </summary>
		public const string Declined = "01";

		/// <summary>
		/// The request is invalid.
		/// </summary>
		public const string InvalidRequest = "02";

		/// <summary>
		/// The amount associated with the request is invalid.
		/// </summary>
		public const string InvalidAmount = "03";

		/// <summary>
		/// The transaction was cancelled, either by the API, the user at the pin pad, or a timeout.
		/// </summary>
		public const string TransactionCancelled = "04";

		/// <summary>
		/// The card details provided were invalid or unsupported.
		/// </summary>
		public const string InvalidCard = "05";

		/// <summary>
		/// The pin pad configuration is missing or invalid.
		/// </summary>

		public const string TerminalConfigurationError = "06";

		/// <summary>
		/// The merchant id specified is not configured, or outside the allowed range.
		/// </summary>
		public const string InvalidMerchant = "07";

		/// <summary>
		/// The reference provided is invalid.
		/// </summary>
		public const string InvalidReference = "08";

		/// <summary>
		/// The transaction requested was not found.
		/// </summary>
		public const string TransactionNotFound = "09";
		
		/// <summary>
		/// The pin pad cannot print it's own receipts, the caller must specific transaction option to send the receipt as part of the response.
		/// </summary>
		public const string MustSupportReceiptPrinting = "10";

		/// <summary>
		/// The amount of the tip entered is too large.
		/// </summary>
		public const string ExcessiveTip = "11";

		/// <summary>
		/// Invalid transaction, mismatched reference.
		/// </summary>
		public const string MismatchedReference = "12";

		/// <summary>
		/// An account type was chosen that is not supported by this card, pin pad or the transaction options specified.
		/// </summary>
		public const string InvalidAccount = "13";

		/// <summary>
		/// There are no transactions available.
		/// </summary>
		public const string NoTransactionsAvailable = "14";

		/// <summary>
		/// The terminal is idle and available to accept requests.
		/// </summary>
		public const string TerminalIdle = "80";

		/// <summary>
		/// The terminal is busy processing other requests, new requests cannot be sent.
		/// </summary>
		public const string TerminalBusy = "81";

		/// <summary>
		/// Terminal fault. 
		/// </summary>
		public const string TerminalFault = "99";
	}
}