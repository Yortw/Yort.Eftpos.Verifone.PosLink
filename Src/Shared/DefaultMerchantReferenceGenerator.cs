using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Generates references uses a thread-safe incrementing counter. The counter value is not stored between sessions and generally this implementation should only be used for test purposes.
	/// </summary>
	public class DefaultMerchantReferenceGenerator : IMerchantReferenceGenerator
	{

		private static DefaultMerchantReferenceGenerator _Instance;
		private static object _Synchroniser = new object();

		private long _Counter;

		private DefaultMerchantReferenceGenerator()
		{
			var now = DateTime.Now;
			var tempStr = now.ToString("yyMMddHHmmss").PadLeft(12);
			tempStr = tempStr.Substring(tempStr.Length - 12, 12);

			Seed(Convert.ToInt64(tempStr));
		}

		/// <summary>
		/// Sets the next id that will be generated to the value of <paramref name="newSeed"/>.
		/// </summary>
		/// <param name="newSeed">The value of the next id to be generated.</param>
		public void Seed(long newSeed)
		{
			var initialCountAtReset = System.Threading.Interlocked.Read(ref _Counter);
			if (newSeed == initialCountAtReset) return;

			if (newSeed > initialCountAtReset)
			{
				while (System.Threading.Interlocked.CompareExchange(ref _Counter, newSeed, System.Threading.Interlocked.Read(ref _Counter)) <= initialCountAtReset)
				{
				}
			}
			else
			{
				while (System.Threading.Interlocked.CompareExchange(ref _Counter, newSeed, System.Threading.Interlocked.Read(ref _Counter)) >= initialCountAtReset)
				{
				}
			}
		}

		/// <summary>
		/// A instance of <see cref="DefaultMerchantReferenceGenerator"/>.
		/// </summary>
		public static DefaultMerchantReferenceGenerator Instance
		{
			get
			{
				if (_Instance != null) return _Instance;

				lock (_Synchroniser)
				{
					return _Instance ?? (_Instance = new DefaultMerchantReferenceGenerator());
				}
			}
		}

		/// <summary>
		/// Returns a new reference that should be unique since the last restart.
		/// </summary>
		/// <returns></returns>
		public string NewReference()
		{
			var newCount = System.Threading.Interlocked.Increment(ref _Counter);

			var retVal = newCount.ToString();
			if (retVal.Length > 12)
			{
				ResetCounter();
				return NewReference();
			}

			return retVal;
		}

		private void ResetCounter()
		{
			Seed(0);
		}
	}
}