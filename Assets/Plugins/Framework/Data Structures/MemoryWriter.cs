using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Framework
{
    public class MemoryWriter : IDisposable
    {
        private MemoryStream _memoryStream;

        public MemoryWriter()
        {
            _memoryStream = new MemoryStream();
        }

        public void Write(byte[] bytes)
        {
            _memoryStream.Write(bytes, 0, bytes.Length);
        }

        public void Write(byte value) => _memoryStream.WriteByte(value);
        public void Write(bool value) => Write(BitConverter.GetBytes(value));
        public void Write(char value) => Write(BitConverter.GetBytes(value));
        public void Write(double value) => Write(BitConverter.GetBytes(value));
        public void Write(short value) => Write(BitConverter.GetBytes(value));
        public void Write(int value) => Write(BitConverter.GetBytes(value));
        public void Write(long value) => Write(BitConverter.GetBytes(value));
        public void Write(float value) => Write(BitConverter.GetBytes(value));
        public void Write(ushort value) => Write(BitConverter.GetBytes(value));
        public void Write(uint value) => Write(BitConverter.GetBytes(value));
        public void Write(ulong value) => Write(BitConverter.GetBytes(value));
        public void Write(string value) => Write(Encoding.Unicode.GetBytes(value));
        public void Write(string value, Encoding encoding) => Write(encoding.GetBytes(value));
        public void Write(Vector3 value) { Write(value.x); Write(value.y); Write(value.z); }
        public void Write(Vector2 value) { Write(value.x); Write(value.y); }
        public void Write(Vector3Int value) { Write(value.x); Write(value.y); Write(value.z); }
        public void Write(Vector2Int value) { Write(value.x); Write(value.y); }
        public void Write(Quaternion value) { Write(value.x); Write(value.y); Write(value.z); Write(value.w); }
        public void Write(DateTime value) => Write(value.ToBinary());

        public byte[] GetBytes(int offset, int length)
        {
            byte[] bytes = new byte[length];
            byte[] buffer = _memoryStream.GetBuffer();

            Buffer.BlockCopy(buffer, offset, bytes, 0, length);

            return bytes;
        }

        public byte[] GetAllBytes()
        {
            byte[] bytes = new byte[_memoryStream.Position];
            byte[] buffer = _memoryStream.GetBuffer();

            Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);

            return bytes;
        }

        public void Dispose()
        {
            _memoryStream.Dispose();
        }
    }

    public class MemoryReader : IDisposable
    {
        private MemoryStream _memoryStream;

        public MemoryReader(byte[] buffer)
        {
            _memoryStream = new MemoryStream(buffer);
        }

        public byte[] ReadBytes(int length)
        {
            byte[] bytes = new byte[length];
            _memoryStream.Read(bytes, 0, length);
            return bytes;
        }

        public byte ReadByte() => (byte)_memoryStream.ReadByte();
        public bool ReadBool() => BitConverter.ToBoolean(ReadBytes(sizeof(bool)), 0);
        public char ReadChar() => BitConverter.ToChar(ReadBytes(sizeof(char)), 0);
        public double ReadDouble() => BitConverter.ToDouble(ReadBytes(sizeof(double)), 0);
        public short ReadShort() => BitConverter.ToInt16(ReadBytes(sizeof(short)), 0);
        public int ReadInt() => BitConverter.ToInt32(ReadBytes(sizeof(int)), 0);
        public long ReadLong() => BitConverter.ToInt64(ReadBytes(sizeof(long)), 0);
        public float ReadFloat() => BitConverter.ToSingle(ReadBytes(sizeof(float)), 0);
        public ushort ReadUShort() => BitConverter.ToUInt16(ReadBytes(sizeof(ushort)), 0);
        public uint ReadUInt() => BitConverter.ToUInt32(ReadBytes(sizeof(uint)), 0);
        public ulong ReadULong() => BitConverter.ToUInt64(ReadBytes(sizeof(ulong)), 0);
        public string ReadString(int length) => Encoding.Unicode.GetString(ReadBytes(length));
        public string ReadString(int length, Encoding encoding) => encoding.GetString(ReadBytes(length));
        public Vector3 ReadVector3() => new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        public Vector2 ReadVector2() => new Vector2(ReadFloat(), ReadFloat());
        public Vector3Int ReadVector3Int() => new Vector3Int(ReadInt(), ReadInt(), ReadInt());
        public Vector2Int ReadVector2Int() => new Vector2Int(ReadInt(), ReadInt());
        public Quaternion ReadQuaternion() => new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        public DateTime ReadDateTime() => DateTime.FromBinary(BitConverter.ToInt64(ReadBytes(sizeof(long)), 0));

        public void Dispose()
        {
            _memoryStream.Dispose();
        }
    }
}