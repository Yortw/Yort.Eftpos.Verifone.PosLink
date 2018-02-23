using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// An attribute used to decorate fields of <see cref="PosLinkRequestMessageBase"/> types indicating how the field is encoded for the protocol.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class PosLinkMessageFieldAttribute : Attribute
	{
		/// <summary>
		/// Sets or returns the format used for the value of this field in the POS Link protocol.
		/// </summary>
		public PosLinkMessageFieldFormat Format { get; set; }
		/// <summary>
		/// Sets or returns the sequence to write this field into the protocol message. Lower values are written first.
		/// </summary>
		public int Sequence { get; set; }
		/// <summary>
		/// Sets or returns the maximum length of this field when encoded into a protocol message. If zero, no maximum length check is applied.
		/// </summary>
		/// <remarks>
		/// <para>If the <see cref="Format"/> property is set to a padded format, such as <see cref="PosLinkMessageFieldFormat.ZeroPaddedNumber"/> then the maximum length is used to determine how many, if any, padding characters are added to the value during formatting.</para>
		/// </remarks>
		public int MaxLength { get; set; }
		/// <summary>
		/// Sets or returns a boolean indicating whether or not this value must be specified in the message.
		/// </summary>
		/// <remarks>
		/// <para>For string values this typically means the string must not be null, empty or contain only whitespace characters.</para>
		/// <para>For nullable types, this typically means the type must not be null.</para>
		/// <para>For non-nullable types this typically means the value must not match the default value for that type.</para>
		/// </remarks>
		public bool Required { get; set; }
	}
}