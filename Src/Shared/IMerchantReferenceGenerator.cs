using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// An interface for components that can generate new, unique merchant references.
	/// </summary>
	public interface IMerchantReferenceGenerator
	{
		/// <summary>
		/// Generates a new reference and returns it as a string.
		/// </summary>
		/// <returns>A string containing a new, unique message reference.</returns>
		string NewReference();
	}
}