using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	internal class PosLinkMessageField
	{
		public PropertyInfo Property { get; set; }
		public PosLinkMessageFieldFormat Format { get; set; }
		public int MaxLength { get; set; }
		public int Sequence { get; set; }
		public bool IsRequired { get; set; }

		internal object GetValue<T>(T message)
		{
			return Property.GetValue(message);
		}
	}
}