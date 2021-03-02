using Styx.Crc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
   public class Protocol
    {
        private static string _startWord = "BEGIN";
        private static byte[] _startBytes = Encoding.ASCII.GetBytes(_startWord);
        private Stream _socket;
        public Protocol(Stream socket)
        {
            _socket = socket;
        }

        public async Task SendAsync (byte[] message)
        {
            if (message == null)
                throw new ArgumentNullException("Failed to send message. The message was null");
            await _socket.WriteAsync(_startBytes, 0, _startBytes.Length);

            var buffer = BitConverter.GetBytes(message.Length);
            await _socket.WriteAsync(buffer, 0, buffer.Length);

            buffer = BitConverter.GetBytes(Crc32.ComputeCrc(buffer));
            await _socket.WriteAsync(buffer, 0, buffer.Length);

            await _socket.WriteAsync(message, 0, message.Length);

            buffer = BitConverter.GetBytes(Crc32.ComputeCrc(message));
            await _socket.WriteAsync(buffer, 0, buffer.Length);
        }

        // ToDo: Test

        public async Task<byte[]> ReceiveAsync ()
        {
            // Read start word
            byte[] startword = Encoding.UTF8.GetBytes(_startWord);
            byte[] buffer = new byte[1];

            for (int i = 0; i < startword.Length; i++)
            {
                await ReceiveAndCheckAsync(buffer);
                if (buffer[0] != startword[i])
                {
                    i = -1;
                }
            }

            // Read message length
            buffer = new byte[BitConverter.GetBytes(uint.MinValue).Length];
            await ReceiveAndCheckAsync(buffer);
 
            int packetDataLength = BitConverter.ToInt32(buffer, 0);
            var crc = Crc32.ComputeCrc(buffer);

            // Read message length CRC
            buffer = new byte[BitConverter.GetBytes(uint.MinValue).Length];
            await ReceiveAndCheckAsync(buffer);

            if (BitConverter.ToUInt32(buffer, 0) != crc)
                throw new ProtocolException(ProtocolException.Error.MessageLengthCRC);

            if (packetDataLength < 0)
                throw new ProtocolException(ProtocolException.Error.NegativeMessageLength);

            if (packetDataLength > 4 * 1024)
                throw new ProtocolException(ProtocolException.Error.TooBigMessageLength);

            // Read message
            buffer = new byte[packetDataLength];
            await ReceiveAndCheckAsync(buffer);

            var message = buffer;

            // Read message CRC
            buffer = new byte[BitConverter.GetBytes(uint.MinValue).Length];
            await ReceiveAndCheckAsync(buffer);

            if (BitConverter.ToUInt32(buffer, 0) != Crc32.ComputeCrc(message))
                throw new ProtocolException(ProtocolException.Error.MessageCRC);

            return message;

        }
        private async Task ReceiveAndCheckAsync (byte[] buffer)
        {
            int lenght = await _socket.ReadAsync(buffer, 0, buffer.Length);

            if (lenght != buffer.Length)
                throw new ProtocolException(ProtocolException.Error.FailedToRead);
        }
    }
}
