using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Provides global and/or default settings for the PosLink interface.
	/// </summary>
	public static class GlobalSettings
	{

		/// <summary>
		/// If true, credit (such as VISA/MasterCard/Amex etc) transactions are allowed. If false, only non-credit EFTPOS transactions are allowed.
		/// </summary>
		/// <remarks>
		/// <para>The default value is true.</para>
		/// </remarks>
		public static bool DefaultAllowCredit { get; set; } = true;

		/// <summary>
		/// If true, asks the terminal to return any EFTPOS receipt as part of the relevant response messages.
		/// </summary>
		/// <remarks>
		/// <para>The default value is true.</para>
		/// </remarks>
		public static bool DefaultReturnReceipt { get; set; } = true;

		private static int _MerchantId = 1;

		/// <summary>
		/// Sets or returns the default merchant id to use for new request messages.
		/// </summary>
		/// <remarks>
		/// <para>The default value is 1.</para>
		/// </remarks>
		/// <exception cref="System.ArgumentException">Thrown if the value provided is less than 1 or greater than 8.</exception>
		public static int DefaultMerchant
		{
			get { return _MerchantId; }
			set
			{
				_MerchantId = value.GuardRange(nameof(value), ProtocolConstants.MinMerchantId, ProtocolConstants.MaxMerchantId);
			}
		}

		private static IMerchantReferenceGenerator _MerchantReferenceGenerator;

		/// <summary>
		/// Sets or returns an implementation of <see cref="IMerchantReferenceGenerator"/> used to generate new default merchant references.
		/// </summary>
		/// <remarks>
		/// <para>The default value, which is also returned if this property is explicitly set to null, is <see cref="DefaultMerchantReferenceGenerator.Instance"/>.</para>
		/// </remarks>
		public static IMerchantReferenceGenerator MerchantReferenceGenerator
		{
			get { return _MerchantReferenceGenerator ?? DefaultMerchantReferenceGenerator.Instance; }
			set
			{
				_MerchantReferenceGenerator = value;
			}
		}

		private static IBufferManager _BufferManager;

		/// <summary>
		/// Sets or returns the <see cref="IBufferManager"/> implementation to be used by the library.
		/// </summary>
		/// <remarks>
		/// <para>The default value is <see cref="BufferManager.Instance"/>, which is also returned if this property is explicitly set to null.</para>
		/// </remarks>
		public static IBufferManager BufferManager
		{
			get { return _BufferManager ?? (_BufferManager = new BufferManager()); }
			set
			{
				_BufferManager = value;
			}
		}

		private static IEftposLogger _Logger;

#pragma warning disable 1574
		/// <summary>
		/// Sets or returns the logging implementation to use.
		/// </summary>
		/// <remarks>
		/// <para>The default value is <see cref="EftposTraceLogger.Instance"/> on platforms that support it (or otherwise <see cref="EftposNullLogger"/>), which is also used if this property is set explicitly to null.</para>
		/// </remarks>
#pragma warning restore 1574
		public static IEftposLogger Logger
		{
			get
			{
#if SUPPORTS_TRACE
				return _Logger ?? EftposTraceLogger.Instance;
#else
				return _Logger ?? EftposNullLogger.Instance;
#endif
			}
			set { _Logger = value; }
		}
	}
}