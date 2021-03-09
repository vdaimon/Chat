using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public static class Serializer
    {
        public static int ReadInt32(this Stream stream)
        {
            byte[] buffer = BitConverter.GetBytes(0);
            int len = stream.Read(buffer, 0, buffer.Length);
            if (len != buffer.Length)
                throw new ArgumentException("Stream doesn't contain enough bytes to read Int32", nameof(stream));
            return BitConverter.ToInt32(buffer, 0);
        }

        public static string ReadString(this Stream stream)
        {
            int len = stream.ReadInt32();
            if (len < 0)
                return null;
            byte[] buffer = new byte[len];
            len = stream.Read(buffer, 0, buffer.Length);
            if (len != buffer.Length)
                throw new ArgumentException("Stream doesn't contain enough bytes to read string", nameof(stream));
            return Encoding.UTF8.GetString(buffer);
        }

        public static void WriteInt32(this Stream stream, int value)
        {
            var buffer = BitConverter.GetBytes(value);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void WriteString(this Stream stream, string value)
        {
            if (value == null)
            {
                stream.WriteInt32(-1);
                return;
            }
            var buffer = Encoding.UTF8.GetBytes(value);
            stream.WriteInt32(buffer.Length);            
            stream.Write(buffer, 0, buffer.Length);
        }
        public static bool ReadBool(this Stream stream)
        {
            var buffer = new byte[1];
            var len = stream.Read(buffer, 0, buffer.Length);
            if (len!=buffer.Length)
                throw new ArgumentException("Stream doesn't contain enough bytes to read bool", nameof(stream));
            return buffer[0] != 0;
        }
        public static void WriteBool(this Stream stream, bool value)
        {
            var buffer = new byte[] { value ? (byte)1 : (byte)0 };
            stream.Write(buffer, 0, buffer.Length);
        }
    }

}
