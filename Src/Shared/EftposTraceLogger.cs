using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
#if SUPPORTS_TRACE
	/// <summary>
	/// A simple log implementation that writes out log entries using <see cref="System.Diagnostics.Trace"/>.
	/// </summary>
	/// <remarks>
	/// <para>Uses the following 'categories' in the trace system:</para>
	/// <list type="bullet">
	/// <item>Eftpos.Error</item>
	/// <item>Eftpos.Warning</item>
	/// <item>Eftpos.Information</item>
	/// <item>Eftpos.Packet</item>
	/// </list>
	/// </remarks>
	public class EftposTraceLogger : IEftposLogger
	{
		private const string Category_Error = "Eftpos.Error";
		private const string Category_Warning = "Eftpos.Warning";
		private const string Category_Information = "Eftpos.Information";
		private const string Category_Packet = "Eftpos.Packet";

		private static IEftposLogger s_Instance;

		/// <summary>
		/// Returns a singleton instance of this logging implementation.
		/// </summary>
		public static IEftposLogger Instance
		{
			get { return s_Instance ?? (s_Instance = new EftposTraceLogger()); }
		}

		private EftposTraceLogger()
		{
		}

		/// <summary>
		/// If true, all incoming and outgoing packet will be written to the log, otherwise packets are not recorded.
		/// </summary>
		public bool LogCommunicationPackets { get; set; }

		/// <summary>
		/// Logs an error.
		/// </summary>
		/// <param name="message">A message or other diagnostic information to include along with an exception.</param>
		/// <param name="exception">The exception representing the error to be logged.</param>
		/// <seealso cref="LogRx(string, byte[], int)"/>
		public void LogError(string message, Exception exception)
		{
			System.Diagnostics.Trace.WriteLine(message + Environment.NewLine + exception?.ToString(), Category_Error);
		}

		/// <summary>
		/// Logs an informational message.
		/// </summary>
		/// <param name="message">The message to be logged.</param>
		public void LogInfo(string message)
		{
			System.Diagnostics.Trace.WriteLine(message, Category_Information);
		}

		/// <summary>
		/// Logs a received packet.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the packet.</param>
		/// <param name="data">A byte array containing the received data.</param>
		/// <param name="dataLength">The length of byte array actually containing the received message.</param>
		/// <seealso cref="LogCommunicationPackets"/>
		public void LogRx(string message, byte[] data, int dataLength)
		{
			if (!LogCommunicationPackets) return;

			System.Diagnostics.Trace.WriteLine(message + Environment.NewLine + "Received: " + System.Text.ASCIIEncoding.ASCII.GetString(data, 0, dataLength) + Environment.NewLine + BytesToHex(data, dataLength), Category_Packet);
		}

		/// <summary>
		/// Log a sent packet.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the packet.</param>
		/// <param name="stream">A <see cref="System.IO.Stream"/> containing the data to be sent.</param>
		/// <seealso cref="LogCommunicationPackets"/>
		public void LogTx(string message, System.IO.Stream stream)
		{
			if (!LogCommunicationPackets) return;
			if (stream == null) return;

			using (var buffer = GlobalSettings.BufferManager.GetBuffer())
			{
				var dataLength = Convert.ToInt32(stream.Length);
				stream.Read(buffer.Bytes, 0, dataLength);
				System.Diagnostics.Trace.WriteLine
				(
					message 
						+ Environment.NewLine 
						+ "Sent: " 
						+ System.Text.ASCIIEncoding.ASCII.GetString(buffer.Bytes, 0, dataLength) 
						+ Environment.NewLine + BytesToHex(buffer.Bytes, dataLength), 
					Category_Packet
				);
			}
		}

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message"></param>
		public void LogWarn(string message)
		{
			System.Diagnostics.Trace.WriteLine(message, Category_Warning);
		}

		/// <summary>
		/// Logs a warning message and an associated exception.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the exception.</param>
		/// <param name="exception">An exception that caused this warning.</param>
		public void LogWarn(string message, Exception exception)
		{
			System.Diagnostics.Trace.WriteLine(message + Environment.NewLine + exception?.ToString(), Category_Warning);
		}

		private static string BytesToHex(byte[] data, int dataLength)
		{
			var sb = new StringBuilder(dataLength * 2);
			for (var cnt = 0; cnt < dataLength; cnt++)
			{
				sb.AppendFormat("{0:X2}", data[cnt]);
			}
			return sb.ToString();
		}
	}

#endif
}