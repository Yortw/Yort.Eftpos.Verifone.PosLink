using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Contains the pin pad response to a <see cref="PollRequestMessage"/>.
	/// </summary>
	/// <remarks>
	/// <para><see cref="PollRequestMessage"/>s are sent automatically by the <see cref="PinpadClient"/> for each request sent throug the <see cref="PinpadClient.ProcessRequest{TRequest, TResponse}(TRequest)"/>. Normally user code does not need to deal with 'poll' messages directly.</para>
	/// </remarks>
	/// <see cref="PollResponseMessage"/>
	public class PollResponseMessage : PosLinkResponseMessageBase
	{
		/// <summary>
		/// Constructs a new message instance from the list of pre-decoded string values received from the pinpad.
		/// </summary>
		/// <param name="fieldValues">The list of values returned from the pinpad in the order specified by the protocol and message type.</param>
		/// <see cref="PollRequestMessage"/>
		public PollResponseMessage(IList<string> fieldValues) : base(fieldValues) { }

		/// <summary>
		/// Returns "POL"
		/// </summary>
		public override string MessageType { get { return ProtocolConstants.MessageType_Poll; } }

		/// <summary>
		/// Returns the status of the device, indicating whether it is ready to accept new requests or not.
		/// </summary>
		public DeviceStatus Status { get { return (DeviceStatus)Convert.ToInt32(Fields[3]); } }

		/// <summary>
		/// Returns any text to be displayed to the user.
		/// </summary>
		public string Display { get { return Fields[4]; } }
	}
}