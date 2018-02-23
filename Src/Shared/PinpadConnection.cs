using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Yort.Eftpos.Verifone.PosLink
{
	internal sealed class PinpadConnection : IDisposable
	{
		private Socket _Socket;
		private System.Net.Sockets.NetworkStream _OutStream;
		private System.Net.Sockets.NetworkStream _InStream;

		public Socket Socket
		{
			get { return _Socket; }
			set
			{
				_Socket = value;
				if (value == null)
				{
					_OutStream = null;
					_InStream = null;
				}
				else
				{
					_InStream = new NetworkStream(Socket, false);
					_OutStream = new NetworkStream(Socket, false);
				}
			}
		}

		public System.IO.Stream OutStream
		{
			get { return _OutStream; }
		}

		public System.IO.Stream InStream
		{
			get { return _InStream; }
		}

		public void Dispose()
		{
			try
			{
				_Socket?.Dispose();
				_Socket = null;
				_InStream = _OutStream = null;
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}

		private static byte[] JunkBuffer = new byte[2];

		internal async Task ClearInputBuffer()
		{
			if (!_InStream.DataAvailable) return;

			var attemptClearBuffer = true;
			while (attemptClearBuffer)
			{
				using (var tcs = new System.Threading.CancellationTokenSource())
				{
					tcs.CancelAfter(ProtocolConstants.Timeout_ClearInputBuffer_Milliseconds);
					try
					{
						attemptClearBuffer = await _InStream.ReadAsync(JunkBuffer, 0, 1, tcs.Token).ConfigureAwait(false) > 0;
					}
					catch (OperationCanceledException)
					{
						attemptClearBuffer = false;
					}
				}	
			}
		}
	}
}