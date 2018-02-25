using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents the possible formats a value may be written into a protocol message as.
	/// </summary>
	public enum PosLinkMessageFieldFormat
	{
		/// <summary>
		/// The value is converted to an invariant string and written as an ASCII string.
		/// </summary>
		Text,
		/// <summary>
		/// The numeric value is converted to an invariant string format and left padded with '0' to it's maximum length.
		/// </summary>
		ZeroPaddedNumber,
		/// <summary>
		/// Returns "Y" if the value is true, otherwise "N".
		/// </summary>
		YesNoBoolean,
		/// <summary>
		/// A date formatted as "DDMMYYYY".
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeCasedCorrectly", MessageId = "Datedd")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MM")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeCasedCorrectly", MessageId = "Myyyy")]
		DateddMMyyyy
	}
}