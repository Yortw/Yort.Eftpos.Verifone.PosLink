using System;
using Yort.Eftpos.Verifone.PosLink;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// A common interface for components that sit between a <see cref="PinpadClient"/> and a user interface view or control and provide communication between them.
	/// </summary>
	public interface IDialogAdapter
	{
		/// <summary>
		/// The caption, if any, to apply to the first button.
		/// </summary>
		string Button1Caption { get; set; }
		/// <summary>
		/// The caption, if any, to apply to the second button.
		/// </summary>
		string Button2Caption { get; set; }
		/// <summary>
		/// The message or prompt to display to the user.
		/// </summary>
		string Message { get; set; }

		/// <summary>
		/// Raised when a receipt needs to be printed (such as a signature slip) during the UI process.
		/// </summary>
		event EventHandler<PrintRequestedEventArgs> PrintRequested;
	}
}