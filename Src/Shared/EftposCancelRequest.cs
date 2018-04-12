using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Requests a in progress EFTPOS transaction be cancelled.
	/// </summary>
	/// <remarks>
	/// <para>The <see cref="PosLinkResponseBase.MerchantReference"/> should be set to the same value as used for the request to be cancelled.</para>
	/// <para>Cancellation can be requested up to the point of pin entry. Cancellation requests should only be sent once, the request will either be processed or ignored based on the device status, repeat sends should not be performed.</para>
	/// <para>There is no matching response message for this request, if the cancellation is successful and immediate Transaction response will be returned with a cancelled status.</para>
	/// <para>See POS Link 2.2 specification, page 12.</para>
	/// </remarks>
	public sealed class EftposCancelRequest : PosLinkTransactionOptionsRequestBase
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public EftposCancelRequest() : base()
		{
		}

		/// <summary>
		/// Returns "CAN".
		/// </summary>
		public override string RequestType
		{
			get { return ProtocolConstants.MessageType_Cancel; }
		}

	}
}