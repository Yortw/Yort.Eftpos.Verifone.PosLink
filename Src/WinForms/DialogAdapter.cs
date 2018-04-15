using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yort.Eftpos.Verifone.PosLink;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Connects a <see cref="PinpadClient"/> instance to a WinForms dialog for display of prompts and queries to a user during an EFTPOS operation.
	/// </summary>
	public sealed class DialogAdapter : INotifyPropertyChanged, IDisposable, IDialogAdapter
	{

		#region Fields

		private PinpadClient _Client;
		private PosLinkDialog _Dialog;
		private IWin32Window _WindowOwner;

		private string _WindowTitle;
		private System.Drawing.Image _Logo;
		private System.Drawing.Color _BackgroundColour;
		private System.Drawing.Color _ForegroundColour;
		private System.Drawing.Font _Font;

		private string _Message;
		private string _Button1Caption;
		private string _Button2Caption;

		private bool _HaveCancelled;
		private bool _CanCancel;
		private bool _QuietMode;

		private QueryOperatorEventArgs _PendingQuery;
		private string _LastPromptMerchantReference;

		#endregion

		#region Events

		/// <summary>
		/// Raised when a property on this object changes. Enables databinding support.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Raised when a receipt (such as a signature slip) needs to be printed as part of a prompt.
		/// </summary>
		public event EventHandler<PrintRequestedEventArgs> PrintRequested;

		#endregion

		#region Constructors

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="client">The <see cref="PinpadClient"/> to handle UI prompts for.</param>
		/// <param name="windowOwner">A <see cref="IWin32Window"/> that will be used as the parent of the dialogs shown.</param>
		/// <param name="quietMode">If true no informational dialog is shown, dialogs will only be shown if the user needs to respond to a prompt (SIG/ASK messages).</param>
		public DialogAdapter(PinpadClient client, IWin32Window windowOwner, bool quietMode)
		{
			if (client == null) throw new ArgumentNullException(nameof(client));

			_Client = client;
			_Dialog = new PosLinkDialog();
			_QuietMode = quietMode;
			_WindowOwner = windowOwner;
			_Font = new Font(_Dialog.Font.FontFamily, 20);
			_BackgroundColour = System.Drawing.SystemColors.Window;
			_ForegroundColour = System.Drawing.SystemColors.ControlText;
			_WindowTitle = "EFTPOS";
			_CanCancel = true;

			_Dialog.dialogAdapterBindingSource.DataSource = this;

			_Dialog.cmdCancel.Click += CmdCancel_Click;
			client.DisplayMessage += Client_DisplayMessage;
			client.QueryOperator += Client_QueryOperator;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Sets or returns the text for the window title bar.
		/// </summary>
		public string WindowTitle
		{
			get { return _WindowTitle; }
			set { SetField(ref _WindowTitle, value); }
		}

		/// <summary>
		/// Sets or returns the background colour of the dialog.
		/// </summary>
		public Color BackgroundColour
		{
			get { return _BackgroundColour; }
			set { SetField(ref _BackgroundColour, value); }
		}

		/// <summary>
		/// Sets or returns the colour of text on thew dialog.
		/// </summary>
		public Color ForegroundColour
		{
			get { return _ForegroundColour; }
			set { SetField(ref _ForegroundColour, value); }
		}

		/// <summary>
		/// Sets or returns an image to be displayed on the dialog.
		/// </summary>
		public Image Logo
		{
			get { return _Logo; }
			set { SetField(ref _Logo, value); }
		}

		/// <summary>
		/// Sets or returns the font to use for the dialog.
		/// </summary>
		public Font Font
		{
			get { return _Font; }
			set { SetField(ref _Font, value); }
		}

		/// <summary>
		/// Sets or returns the message currently displayed to the user.
		/// </summary>
		public string Message { get { return _Message; } set { SetField(ref _Message, value); } }

		/// <summary>
		/// Sets or returns the option for the first button available during an operator query.
		/// </summary>
		public string Button1Caption
		{
			get { return _Button1Caption; }
			set
			{
				if (SetField(ref _Button1Caption, value))
				{
					OnPropertyChanged(nameof(Button1Visible));
					OnPropertyChanged(nameof(CancelButtonVisible));
				}
			}
		}

		/// <summary>
		/// Sets or returns the option for the second button available during an operator query.
		/// </summary>
		public string Button2Caption
		{
			get { return _Button2Caption; }
			set
			{
				if (SetField(ref _Button2Caption, value))
					OnPropertyChanged(nameof(Button2Visible));
			}
		}

		/// <summary>
		/// Returns true if the <see cref="Button1Caption"/> is not null or empty string.
		/// </summary>
		public bool Button1Visible { get { return !String.IsNullOrEmpty(_Button1Caption); } }
		/// <summary>
		/// Returns true if the <see cref="Button2Caption"/> is not null or empty string.
		/// </summary>
		public bool Button2Visible { get { return !String.IsNullOrEmpty(_Button1Caption); } }

		/// <summary>
		/// Returns true if the <see cref="Button1Caption"/> is null or empty string.
		/// </summary>
		public bool CancelButtonVisible { get { return _CanCancel && !_HaveCancelled && !Button1Visible; } }

		#endregion

		#region Public Methods
		
		/// <summary>
		/// Hides the UI/dialog if it is currently visible.
		/// </summary>
		/// <remarks>
		/// <para>The UI/dialog will be automatically re-displayed if another display or query prompt is requested.</para>
		/// </remarks>		
		public void Hide()
		{
			var dialog = _Dialog;
			if (dialog == null) return;
			if (!dialog.IsHandleCreated) return;

			try
			{
				if (dialog.InvokeRequired)
				{
					dialog.Invoke(new Action(Hide));
					return;
				}

				dialog?.Hide();
			}
			catch (InvalidOperationException) { } // Race condition on dialog disposal etc.
		}

		#endregion

		#region Private Methods

		private void DisconnectQueryButtonHandlers()
		{
			_Dialog.cmdButton1.Click -= CmdButton1_Click;
			_Dialog.cmdButton2.Click -= CmdButton2_Click;
		}

		private void OnPrintRequested(string receiptText)
		{
			PrintRequested?.Invoke(this, new PrintRequestedEventArgs(receiptText));
		}

		private bool SetField<T>(ref T backingField, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;

			backingField = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ShowIfNotVisible()
		{
			if (!_Dialog.Visible)
			{
				_Dialog.StartPosition = _WindowOwner != null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;

				_Dialog.Show(_WindowOwner);
				_Dialog.Center(); // FML, StartupPosition = CenterToParent not working; https://stackoverflow.com/questions/8567058/formstartposition-centerparent-does-not-work
			}

			_Dialog.BringToFront();
			_Dialog.Activate();
		}

		private void DisposeDialog(PosLinkDialog dialog)
		{
			dialog?.Dispose();
		}

		#endregion

		#region Event Handlers

		private void Client_DisplayMessage(object sender, DisplayMessageEventArgs e)
		{
			if (_QuietMode) return;

			if (_Dialog.InvokeRequired)
			{
				_Dialog.Invoke(new Action<object, DisplayMessageEventArgs>(this.Client_DisplayMessage), sender, e);
				return;
			}

			ShowIfNotVisible();

			_PendingQuery = null;
			_LastPromptMerchantReference = e.Message?.MerchantReference;
			this.Message = e.Message.MessageText;
			this.Button1Caption = null;
			this.Button2Caption = null;

			//Oh how I wish there was a better way!
			//Can't cancel from pin entry screen or later part of process.
			if (_CanCancel && String.Equals(e.Message.MessageText, "AWAITING PIN"))
				_CanCancel = false;

			OnPropertyChanged(nameof(CancelButtonVisible));
		}

		private void Client_QueryOperator(object sender, QueryOperatorEventArgs e)
		{
			if (_Dialog.InvokeRequired)
			{
				_Dialog.Invoke(new Action<object, QueryOperatorEventArgs>(this.Client_QueryOperator), sender, e);
				return;
			}

			ShowIfNotVisible();

			OnPrintRequested(e.ReceiptText);

			_PendingQuery = e;

			this.Message = e.Prompt;
			this.Button1Caption = e.AllowedResponses[0];
			if (e.AllowedResponses.Count > 1)
				this.Button2Caption = e.AllowedResponses[1];
			else
				this.Button2Caption = null;

			_Dialog.cmdButton1.Click -= CmdButton1_Click;
			_Dialog.cmdButton1.Click += CmdButton1_Click;
			_Dialog.cmdButton2.Click -= CmdButton2_Click;
			_Dialog.cmdButton2.Click += CmdButton2_Click;
			OnPropertyChanged(nameof(CancelButtonVisible));
		}

		private async void CmdCancel_Click(object sender, EventArgs e)
		{
			if (_HaveCancelled) return;

			try
			{
				var reference = _PendingQuery?.MerchantReference ?? _LastPromptMerchantReference;
				if (!String.IsNullOrEmpty(reference))
				{
					await _Client.ProcessRequest<EftposCancelRequest, TransactionResponseBase>(new EftposCancelRequest() { MerchantReference = reference });
					_HaveCancelled = true;
					OnPropertyChanged(nameof(CancelButtonVisible));
				}
			}
			catch { }
		}

		private void CmdButton2_Click(object sender, EventArgs e)
		{
			DisconnectQueryButtonHandlers();
			_PendingQuery.SetResponse(_PendingQuery.AllowedResponses[1]);
		}

		private void CmdButton1_Click(object sender, EventArgs e)
		{
			DisconnectQueryButtonHandlers();
			_PendingQuery.SetResponse(_PendingQuery.AllowedResponses[0]);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes this object and all internal resources, disconnects from the <see cref="PinpadClient"/> events and disconnects any event handlers connected to itself.
		/// </summary>
		public void Dispose()
		{
			try
			{
				_Client.DisplayMessage -= Client_DisplayMessage;
				_Client.QueryOperator -= Client_QueryOperator;
				_Dialog.cmdCancel.Click -= CmdCancel_Click;

				var dialog = _Dialog;
				if (dialog != null && !dialog.IsDisposed)
				{
					if (dialog.InvokeRequired)
						dialog.Invoke(new Action<PosLinkDialog>(DisposeDialog), dialog);
					else
						DisposeDialog(dialog);
				}
				_Font?.Dispose();
				PrintRequested = null;
				PropertyChanged = null;
				_WindowOwner = null;
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}

		#endregion

	}
}