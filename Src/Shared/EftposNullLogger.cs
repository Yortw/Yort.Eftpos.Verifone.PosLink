using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
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
	public class EftposNullLogger : IEftposLogger
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
			get { return s_Instance ?? (s_Instance = new EftposNullLogger()); }
		}

		private EftposNullLogger()
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public bool LogCommunicationPackets { get; set; }

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="message">A message or other diagnostic information to include along with an exception.</param>
		/// <param name="exception">The exception representing the error to be logged.</param>
		/// <seealso cref="LogRx(string, byte[], int)"/>
		public void LogError(string message, Exception exception)
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="message">The message to be logged.</param>
		public void LogInfo(string message)
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the packet.</param>
		/// <param name="data">A byte array containing the received data.</param>
		/// <param name="dataLength">The length of byte array actually containing the received message.</param>
		/// <seealso cref="LogCommunicationPackets"/>
		public void LogRx(string message, byte[] data, int dataLength)
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the packet.</param>
		/// <param name="stream">A stream containing the data to be sent.</param>
		/// <seealso cref="LogCommunicationPackets"/>
		public void LogTx(string message, System.IO.Stream stream)
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to be logged as a warning.</param>
		public void LogWarn(string message)
		{
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the exception.</param>
		/// <param name="exception">An exception that caused this warning.</param>
		public void LogWarn(string message, Exception exception)
		{
		}

	}
}