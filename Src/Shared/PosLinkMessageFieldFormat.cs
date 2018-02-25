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
		DateDdMmYyyy
	}
}