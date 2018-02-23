using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Event arguments for the <see cref="PinpadClient.DisplayMessage"/> event.
	/// </summary>
	public class DisplayMessageEventArgs : EventArgs
	{
		private readonly DisplayMessage _Message;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="message">A <see cref="DisplayMessage"/> containing details of the message to be displayed.</param>
		/// <exception cref="System.ArgumentNullException">Thrown if <paramref name="message"/> is null.</exception>
		public DisplayMessageEventArgs(DisplayMessage message)
		{
			_Message = message.GuardNull(nameof(message));
		}

		/// <summary>
		/// Returns a <see cref="DisplayMessage"/> instance containing details of the message to be displayed.
		/// </summary>
		public DisplayMessage Message { get { return _Message; } }
	}
} 