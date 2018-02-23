using System;
using System.Collections.Generic;
using System.Text;

namespace Yort.Eftpos.Verifone.PosLink
{
	internal sealed class DataBuffer : IDisposable
	{

		internal DataBuffer(ArraySegment<byte> data)
		{
			_Data = data;
		}

		private ArraySegment<byte> _Data;

		public byte[] Bytes { get { return _Data.Array; } }

		public int Length { get { return _Data.Count; } }

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}