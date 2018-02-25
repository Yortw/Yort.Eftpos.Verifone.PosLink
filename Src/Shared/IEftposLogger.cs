using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// An interface for logging components used by the Yort.Eftpos.Verifone.PosLink library.
	/// </summary>
	public interface IEftposLogger
	{

		/// <summary>
		/// Logs an error.
		/// </summary>
		/// <param name="message">A message or other diagnostic information to include along with an exception.</param>
		/// <param name="exception">The exception representing the error to be logged.</param>
		/// <seealso cref="LogRx(string, byte[], int)"/>
		void LogError(string message, Exception exception);

		/// <summary>
		/// Logs an informational message.
		/// </summary>
		/// <param name="message">The message to be logged.</param>
		void LogInfo(string message);

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to be logged as a warning.</param>
		void LogWarn(string message);

		/// <summary>
		/// Logs a warning message and an associated exception.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the exception.</param>
		/// <param name="exception">An exception that caused this warning.</param>
		void LogWarn(string message, Exception exception);

		/// <summary>
		/// Log a sent packet.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the packet.</param>
		/// <param name="stream">A stream containing the bytes to be sent.</param>
		/// <seealso cref="LogCommunicationPackets"/>
		void LogTx(string message, System.IO.Stream stream);

		/// <summary>
		/// Logs a received packet.
		/// </summary>
		/// <param name="message">An informational or diagnostic message to include with the packet.</param>
		/// <param name="data">A byte array containing the received data.</param>
		/// <param name="dataLength">The length of byte array actually containing the received message.</param>
		/// <seealso cref="LogCommunicationPackets"/>
		void LogRx(string message, byte[] data, int dataLength);

		/// <summary>
		/// If true, all incoming and outgoing packet will be written to the log, otherwise packets are not recorded.
		/// </summary>
		/// <remarks>
		/// <para>The default value should be false for all implementations.</para>
		/// </remarks>
		bool LogCommunicationPackets { get; set; }

	}
}