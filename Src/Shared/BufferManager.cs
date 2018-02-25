using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// The default buffer manager implementation. Uses <see cref="Microsoft.IO.RecyclableMemoryStream"/> streams for both in memory streams and temporary byte array buffers, ensuring pooling of the backing byte arrays.
	/// </summary>
	public class BufferManager : IBufferManager
	{

		private readonly Microsoft.IO.RecyclableMemoryStreamManager _RecyclableMemoryStreamManager = new Microsoft.IO.RecyclableMemoryStreamManager
		(
			ProtocolConstants.MaxBufferSize_Read,
			Microsoft.IO.RecyclableMemoryStreamManager.DefaultLargeBufferMultiple,
			Microsoft.IO.RecyclableMemoryStreamManager.DefaultMaximumBufferSize
		);

		private static IBufferManager s_Instance;

		/// <summary>
		/// Returns a singleton instance of the buffer manager.
		/// </summary>
		public static IBufferManager Instance
		{
			get { return s_Instance ?? (s_Instance = new BufferManager()); }
		}

		/// <summary>
		/// Returns a new <see cref="DataBuffer"/> of the maximum size of a protocol message.
		/// </summary>
		/// <returns>Returns a new <see cref="DataBuffer"/> instance.</returns>
		public DataBuffer GetBuffer()
		{
			var stream = _RecyclableMemoryStreamManager.GetStream();
			try
			{
				return new DataBuffer
				(
					stream.GetBuffer(),
					stream
				);
			}
			catch
			{
				stream?.Dispose();
				throw;
			}
		}

		/// <summary>
		/// Returns a pooled <see cref="Microsoft.IO.RecyclableMemoryStream"/> that can be used in place of a memory stream.
		/// </summary>
		/// <returns>A <see cref="Microsoft.IO.RecyclableMemoryStream"/> instance.</returns>
		public Stream GetStream()
		{
			return _RecyclableMemoryStreamManager.GetStream();
		}

	}
}