using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// A request to poll the device for it's status. Used to determine if the device is currently busy before sending a new request.
	/// </summary>
	/// <see cref="PollRequestMessage"/>
	public sealed class PollRequestMessage : PosLinkRequestMessageBase
	{
		/// <summary>
		/// Returns the string "POL".
		/// </summary>
		public override string RequestType { get { return ProtocolConstants.MessageType_Poll; } }

		/// <summary>
		/// Sets or returns the number of the merchant account to conduct the transaction under.
		/// </summary>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 3, Required = true, Sequence = 2)]
		public int MerchantNumber { get; set; } = GlobalSettings.MerchantId;

		/// <summary>
		/// The version of the protocol in use.
		/// </summary>
		/// <remarks>
		/// <para>Default value is 2.2</para>
		/// </remarks>
		[PosLinkMessageField(Format = PosLinkMessageFieldFormat.Text, MaxLength = 10, Required = true, Sequence = 3)]
		public string SpecVersion { get; set; } = ProtocolConstants.ProtocolVersion_2Dot2;

		/// <summary>
		/// Validates the properties of this message contain valid values.
		/// </summary>
		/// <remarks>
		/// <para>In addition to validations provided by the base class;</para>
		/// <list type="bullet">
		/// <item>Ensures the <see cref="MerchantNumber"/> is not null, empty or only whitespace.</item>
		/// <item>Ensures the <see cref="SpecVersion"/> is not null, empty or only whitespace.</item>
		/// </list>
		/// </remarks>
		public override void Validate()
		{
			MerchantNumber.GuardRange(nameof(MerchantNumber), ProtocolConstants.MinMerchantId, ProtocolConstants.MaxMerchantId);
			SpecVersion.GuardNullOrWhiteSpace(nameof(SpecVersion));

			base.Validate();
		}
	}
}