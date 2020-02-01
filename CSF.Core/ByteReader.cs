using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CSF.Core
{
    public class ByteReader : Stream
	{
		protected int BaseOffset;
		protected long Pos;
		protected long Size;
		readonly bool _isBuffered;
		byte[] _buff;
		bool _isBufferInitialized;
		public ByteReader(Stream baseStream, int baseOffset, long fileSize, bool isBuffered = false)
		{
			Size = fileSize;
			BaseOffset = baseOffset;
			BaseStream = baseStream;
			_isBuffered = isBuffered;
		}

		public ByteReader(Stream baseStream, bool isBuffered = false)
		{
			BaseStream = baseStream;
			BaseOffset = 0;
			Size = baseStream.Length;
			_isBuffered = isBuffered;
		}

		public Stream BaseStream { get; internal protected set; }
		public override bool CanRead => Pos < Size;

		public override bool CanSeek => true;

		public override bool CanWrite => false;

		public bool Eof => Remaining <= 0;

		public override long Length => Size;

		public override long Position
		{
			get => Pos;
			set
			{
				Pos = value;
				if (!_isBuffered && Pos + BaseOffset != BaseStream.Position)
					BaseStream.Seek(Pos + BaseOffset, SeekOrigin.Begin);
			}
		}

		public long Remaining => Length - Pos;

		public override void Close()
		{
			base.Close();
			BaseStream.Close();
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			count = Math.Min(count, (int)(Length - Position));
			if (_isBuffered)
			{
				if (!_isBufferInitialized)
					InitBuffer();

				Array.Copy(_buff, Pos, buffer, offset, count);
			}
			else
			{
				// ensure
				BaseStream.Position = BaseOffset + Pos;
				BaseStream.Read(buffer, offset, count);
			}
			Pos += count;
			return count;
		}

		public byte[] Read(int numBytes)
		{
			var ret = new byte[numBytes];
			Read(ret, 0, numBytes);
			return ret;
		}

		public new byte ReadByte()
		{
			return ReadUInt8();
		}

		public string ReadCString(int count)
		{
			var arr = Read(count);
			var sb = new StringBuilder();
			int i = 0;
			while (i < count && arr[i] != 0)
				sb.Append((char)arr[i++]);
			return sb.ToString();
		}
		public double ReadDouble()
		{
			return BitConverter.ToDouble(Read(sizeof(Double)), 0);
		}

		public float ReadFloat()
		{
			return BitConverter.ToSingle(Read(sizeof(Single)), 0);
		}

		public float ReadFloat2()
		{
			var ori = Read(sizeof(Single));
			byte[] rev = new[] { ori[3], ori[2], ori[1], ori[0] };
			return BitConverter.ToSingle(rev, 0);
		}

		public short ReadInt16()
		{
			return BitConverter.ToInt16(Read(sizeof(Int16)), 0);
		}

		public int ReadInt32()
		{
			return BitConverter.ToInt32(Read(sizeof(Int32)), 0);
		}

		public sbyte ReadSByte()
		{
			return unchecked((sbyte)ReadUInt8());
		}

		public sbyte[] ReadSigned(int numBytes)
		{
			var b = new byte[numBytes];
			Read(b, 0, numBytes);
			sbyte[] ret = new sbyte[numBytes];
			Buffer.BlockCopy(b, 0, ret, 0, b.Length);
			return ret;
		}

		public ushort ReadUInt16()
		{
			return BitConverter.ToUInt16(Read(sizeof(UInt16)), 0);
		}

		public uint ReadUInt32()
		{
			return BitConverter.ToUInt32(Read(sizeof(UInt32)), 0);
		}

		public byte ReadUInt8()
		{
			return Read(1)[0];
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position += offset;
					break;
				case SeekOrigin.End:
					Position = Length - offset;
					break;
			}
			return Position;
		}

		public override void SetLength(long value)
		{
			Size = value;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		private void InitBuffer()
		{
			// ensure
			BaseStream.Position = BaseOffset + Pos;
			_buff = new byte[Size];
			BaseStream.Read(_buff, 0, (int)Size);
			_isBufferInitialized = true;
		}
	}
}
