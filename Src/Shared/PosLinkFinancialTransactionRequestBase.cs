using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents a transaction request that contains an amount.
	/// </summary>
	public abstract class PosLinkFinancialTransactionRequestBase : PosLinkTransactionOptionsRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		protected PosLinkFinancialTransactionRequestBase() : base()
		{
		}

		/// <summary>
		/// Sets or returns the core amount of the transaction (as opposed to any secondary amount, such as cash out or tip).
		/// </summary>
		/// <remarks>
		/// <para>For a purchase or refund transaction this is the amount of the purchase/refund. For cash out only transaction, this is the amount of cash out.</para>
		/// <para>Must not be negative.</para>
		/// </remarks>
		public abstract decimal Amount { get; set; }

		/// <summary>
		/// Validates that <see cref="Amount"/> is a non-zero, positive value in addition to validations provded by <see cref="PosLinkRequestBase.Validate"/>.
		/// </summary>
		public override void Validate()
		{
			Amount.GuardZeroOrNegative(nameof(Amount));

			base.Validate();
		}
	}
}
