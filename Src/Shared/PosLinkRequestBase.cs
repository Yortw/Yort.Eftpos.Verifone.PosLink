using System;
using System.Collections.Generic;
using System.Text;
using Ladon;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Base class for all classes that represent messages to be sent via the POS Link protocol.
	/// </summary>
	public abstract class PosLinkRequestBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected PosLinkRequestBase()
		{
		}

		/// <summary>
		/// The unique reference for this transaction, used to correlate requests &amp; replies and ensure idempotent responses.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 12, Required = true, Sequence = 0)]
		public string MerchantReference { get; set; } = GlobalSettings.MerchantReferenceGenerator?.NewReference();

		/// <summary>
		/// The message type used by the protocol to indicate the type of request being made.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 1)]
		public abstract string RequestType
		{
			get;
		}

		/// <summary>
		/// Returns the merchant id associated with this response and it's request.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 1, Required = true, Sequence = 2)]
		public int Merchant { get; set; } = GlobalSettings.MerchantId;

		/// <summary>
		/// Called to ensure all properties are set to valid values. If any value is invalid a <see cref="System.ArgumentException"/> (or derived exception) should be thrown.
		/// </summary>
		/// <remarks>
		/// <para>Maximum lengths and requiredness are checked by the message writer as fields are processed. This method need only perform additional or more complex validations.</para>
		/// <para>Performs the following additioanl checks</para>
		/// <list type="bullet">
		/// <item><see cref="MerchantReference"/> must not be null, empty or only whitespace.</item>
		/// <item><see cref="RequestType"/> must not be null, empty or only whitespace.</item>
		/// <item><see cref="Merchant"/> is between 1 and 8.</item>
		/// </list>
		/// </remarks>
		public virtual void Validate()
		{
			MerchantReference.GuardNullOrWhiteSpace(nameof(MerchantReference));
			RequestType.GuardNullOrWhiteSpace(nameof(RequestType));
			Merchant.GuardRange(nameof(Merchant), ProtocolConstants.MinMerchantId, ProtocolConstants.MaxMerchantId);
		}
	}
}