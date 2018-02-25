using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Base class for all classes that represent messages that contain transaction options.
	/// </summary>
	public abstract class PosLinkTransactionRequestBase : PosLinkRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected PosLinkTransactionRequestBase()
		{
		}

		/// <summary>
		/// Sets or returns the number of the merchant account to conduct the transaction under.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 2)]
		public int MerchantNumber { get; set; } = GlobalSettings.MerchantId;

		/// <summary>
		/// Returns the transaction options string required for the protocol based on the <see cref="ReturnReceipt"/> and <see cref="AllowCredit"/> properties.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 2, Required = true, Sequence = int.MaxValue)]
		public string TransactionOptions
		{
			get
			{
				return new string(new char[] { BooleanToYesNo(this.ReturnReceipt), BooleanToYesNo(this.AllowCredit) });
			}
		}

		/// <summary>
		/// If true, asks the terminal to return any EFTPOS receipt as part of the relevant response messages.
		/// </summary>
		public bool ReturnReceipt { get; set; } = GlobalSettings.ReturnReceipt;

		/// <summary>
		/// If true, credit (such as VISA/MasterCard/Amex etc) transactions are allowed. If false, only non-credit EFTPOS transactions are allowed.
		/// </summary>
		public bool AllowCredit { get; set; } = GlobalSettings.AllowCredit;

		/// <summary>
		/// Validates the properties of this message contain valid values.
		/// </summary>
		/// <remarks>
		/// <para>In addition to validations provided by the base class;</para>
		/// <list type="bullet">
		/// <item>Ensures the <see cref="MerchantNumber"/> is not null, empty or only whitespace.</item>
		/// </list>
		/// </remarks>
		public override void Validate()
		{
			MerchantNumber.GuardRange(nameof(MerchantNumber), ProtocolConstants.MinMerchantId, ProtocolConstants.MaxMerchantId);
			base.Validate();
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
		private static char BooleanToYesNo(bool value)
		{
			return value ? 'Y' : 'N';
		}

	}
}