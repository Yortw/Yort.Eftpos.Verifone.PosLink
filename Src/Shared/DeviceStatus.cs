using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Represents the possible device status values returned from a <see cref="PollRequest"/> response.
	/// </summary>
	public enum DeviceStatus
	{
		/// <summary>
		/// The device is ready to accept requests.
		/// </summary>
		Ready = 80,
		/// <summary>
		/// The device is busy processing a transaction already.
		/// </summary>
		Busy = 81
	}
}