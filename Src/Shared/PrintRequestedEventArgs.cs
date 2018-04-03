using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Event arguments for the <see cref="Yort.Eftpos.Verifone.PosLink.IDialogAdapter.PrintRequested"/> event.
	/// </summary>
	public class PrintRequestedEventArgs : EventArgs
	{

		private readonly string _ReceiptText;

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="receiptText">The text to be printed.</param>
		public PrintRequestedEventArgs(string receiptText)
		{
			_ReceiptText = receiptText;
		}

		/// <summary>
		/// Returns the text to be printed.
		/// </summary>
		public string ReceiptText => _ReceiptText;
	}
}