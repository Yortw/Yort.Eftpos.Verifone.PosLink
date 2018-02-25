using Ladon;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Holds a byte array and a length property indicating how much of the array is actually used. Can be disposed, allowing for convenient object pooling.
	/// </summary>
	public sealed class DataBuffer : IDisposable
	{
		private byte[] _Data;
		private int _Length;

		private IDisposable _Disposable;

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="buffer">The byte array to wrap. The full length of the array will be assumed to be used.</param>
		/// <param name="disposable">An object implementing <see cref="IDisposable"/> used to return the buffer to a pool when this instance is disposed.</param>
		public DataBuffer(byte[] buffer, IDisposable disposable) : this(buffer.GuardNull(nameof(buffer)), buffer.Length, disposable)
		{
		}

		/// <summary>
		/// Full constructor.
		/// </summary>
		/// <param name="buffer">The byte array to wrap. The full length of the array will be assumed to be used.</param>
		/// <param name="length">The amount of the byte array that has actually been filed with useful data.</param>
		/// <param name="disposable">An object implementing <see cref="IDisposable"/> used to return the buffer to a pool when this instance is disposed.</param>
		public DataBuffer(byte[] buffer, int length, IDisposable disposable) : this(buffer, length)
		{
			_Disposable = disposable;
		}

		/// <summary>
		/// Partial constructor.
		/// </summary>
		/// <param name="buffer">The byte array to wrap. The full length of the array will be assumed to be used.</param>
		/// <param name="length">The amount of the byte array that has actually been filed with useful data.</param>
		public DataBuffer(byte[] buffer, int length)
		{
			_Data = buffer;
			_Length = length;
		}

		/// <summary>
		/// Returns the byte array associated with this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
		public byte[] Bytes { get { return _Data; } }
		
		/// <summary>
		/// Returns the number of bytes in <see cref="Bytes"/> that have been filled with useful data.
		/// </summary>
		public int Length { get { return _Length; } }

		/// <summary>
		/// Disposes this instance and the disposable value passed to the constructor, if any. If the buffer is poolable, this should return it to the pool.
		/// </summary>
		public void Dispose()
		{
			try
			{
				_Disposable?.Dispose();
			}
			finally
			{
				GC.SuppressFinalize(this);
			}
		}
	}
}