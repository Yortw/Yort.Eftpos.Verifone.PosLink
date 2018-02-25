using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	/// <summary>
	/// Interface for components that can create and/or manage a pool of streams and buffers.
	/// </summary>
	public interface IBufferManager
	{
		/// <summary>
		/// Returns a <see cref="System.IO.Stream"/> implementation that can be used in place of a <see cref="System.IO.MemoryStream"/> for temporary serialisation/deserialisation of data.
		/// </summary>
		/// <returns>A type derived from <see cref="System.IO.Stream"/>.</returns>
		System.IO.Stream GetStream();

		/// <summary>
		/// Returns a new <see cref="DataBuffer"/> of the maximum size of a protocol message.
		/// </summary>
		/// <returns>Returns a new <see cref="DataBuffer"/> instance.</returns>
		DataBuffer GetBuffer();
	}
}