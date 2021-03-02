using ChatProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Styx.Crc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocolTests
{
    [TestClass]
    public class ProtocolTests
    {
        private enum SerializeError
        {
            None,
            NegativeMessageLength,
            TooBigMessageLength,
            MessageLengthCRC,
            MessageCRC,
        } 

        private byte[] Serialize(byte[] message, SerializeError error= SerializeError.None)
        {
            byte[] messLen = BitConverter.GetBytes(error == SerializeError.NegativeMessageLength ?
                -1 : error == SerializeError.TooBigMessageLength ?
                int.MaxValue : message.Length);
            var buffer = new byte[] { };
            return buffer.Concat(Encoding.UTF8.GetBytes("BEGIN"))
                  .Concat(messLen)
                  .Concat(error == SerializeError.MessageLengthCRC ? new byte[] {0x55,0x11,0x33,0x44 } : BitConverter.GetBytes(Crc32.ComputeCrc(messLen)))
                  .Concat(message)
                  .Concat(error == SerializeError.MessageCRC ? new byte[] { 0x55, 0x11, 0x33, 0x44 } : BitConverter.GetBytes(Crc32.ComputeCrc(message)))
                  .ToArray();
        }

        [TestMethod]
        public void SendAsyncEmpty()
        {
            byte[] message = new byte[] { };
            var expRes = Serialize(message);
            
            using (MemoryStream stream = new MemoryStream())
            {
                Protocol protocol = new Protocol(stream);
                Task t = protocol.SendAsync(message);
                t.Wait();
                Assert.IsTrue(t.IsCompleted);
                CollectionAssert.AreEqual(expRes, stream.ToArray());
            }      
        }

        [TestMethod]
        public void SendAsyncNotEmpty()
        {
            byte[] message = Encoding.UTF8.GetBytes("Hello world");
            var expRes = Serialize(message);

            using (MemoryStream stream = new MemoryStream())
            {
                Protocol protocol = new Protocol(stream);
                Task t = protocol.SendAsync(message);
                t.Wait();
                Assert.IsTrue(t.IsCompleted);
                CollectionAssert.AreEqual(expRes, stream.ToArray());
            }
        }

        [TestMethod]
        public async Task SendAsyncNull()
        {
            byte[] message = null;

            using (MemoryStream stream = new MemoryStream())
            {
                Protocol protocol = new Protocol(stream);

                await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
                {
                    await protocol.SendAsync(message);
                });
            }
        }

        [TestMethod]
        public async Task ReceiveAsyncNotEmptyValid()
        {
            string text = "Hello, world!";
            byte[] message = Encoding.UTF8.GetBytes(text);
    
            using (MemoryStream stream = new MemoryStream(Serialize(message)))
            {
                Protocol protocol = new Protocol(stream);
                byte[] receive = await protocol.ReceiveAsync();
                CollectionAssert.AreEqual(message, receive);
            }
        }

        [TestMethod]
        public async Task ReceiveAsyncEmptyValid()
        {
            byte[] message = new byte[] { };

            using (MemoryStream stream = new MemoryStream(Serialize(message)))
            {
                Protocol protocol = new Protocol(stream);
                byte[] receive = await protocol.ReceiveAsync();

                CollectionAssert.AreEqual(message, receive);
            }
        }

        [TestMethod]
        public async Task ReceiveAsyncMultipleValid()
        {
            string text = "Hello, world!";
            byte[] packet = Serialize(Encoding.UTF8.GetBytes(text));
            var messCollection = new byte[] { };

            for (int i = 0; i < 3; i++)
            {
                messCollection = messCollection.Concat(packet).ToArray();
            }

            using (MemoryStream stream = new MemoryStream(messCollection))
            {
                Protocol protocol = new Protocol(stream);

                for (int i = 0; i < 3; i++)
                {
                    byte[] receive = await protocol.ReceiveAsync();
                    CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(text), receive);
                }
            }
        }

        [TestMethod]
        public async Task ReceiveAsyncRandomMultiple()
        {
            string text = "Hello, world!";
            byte[] packet = Serialize(Encoding.UTF8.GetBytes(text));
            var messCollection = new byte[] { };

            messCollection = messCollection.Concat(packet)
                             .Concat(Serialize(Encoding.UTF8.GetBytes(text), SerializeError.NegativeMessageLength))
                             .Concat(packet)
                             .Concat(Serialize(Encoding.UTF8.GetBytes(text), SerializeError.TooBigMessageLength))
                             .Concat(packet)
                             .Concat(Serialize(Encoding.UTF8.GetBytes(text), SerializeError.MessageLengthCRC))
                             .Concat(packet)
                             .Concat(Serialize(Encoding.UTF8.GetBytes(text), SerializeError.MessageCRC))
                             .Concat(packet)
                             .ToArray();

            using (MemoryStream stream = new MemoryStream(messCollection))
            {
                Protocol protocol = new Protocol(stream);

                for (int i = 1; i < 10; i++)
                {

                    if (i % 2 != 0)
                    {
                        byte[] receive = await protocol.ReceiveAsync();
                        CollectionAssert.AreEqual(Encoding.UTF8.GetBytes(text), receive);
                    }
                    else
                    {
                        await Assert.ThrowsExceptionAsync<ProtocolException>(async () =>
                        {
                            await protocol.ReceiveAsync();
                        });
                    }
                }
            }
        }
                
        [TestMethod]
        public async Task ReceiveAsyncInvalidMessageLengthCRC()
        {
            byte[] message = new byte[] { };

            using (MemoryStream stream = new MemoryStream(Serialize(message, SerializeError.MessageLengthCRC)))
            {
                var ex = await Assert.ThrowsExceptionAsync<ProtocolException>(async () =>
                {
                    Protocol protocol = new Protocol(stream);
                    byte[] receive = await protocol.ReceiveAsync();
                });
                Assert.AreEqual(ProtocolException.Error.MessageLengthCRC, ex.ErrorType);

            }
        }

        [TestMethod]
        public async Task ReceiveAsyncInvalidMessageCRC()
        {
            byte[] message = new byte[] { };

            using (MemoryStream stream = new MemoryStream(Serialize(message, SerializeError.MessageCRC)))
            {
                var ex = await Assert.ThrowsExceptionAsync<ProtocolException>(async () =>
                {
                    Protocol protocol = new Protocol(stream);
                    byte[] receive = await protocol.ReceiveAsync();
                });
                Assert.AreEqual(ProtocolException.Error.MessageCRC, ex.ErrorType);

            }
        }

        [TestMethod]
        public async Task ReceiveAsyncNegativeMessageLength()
        {
            byte[] message = new byte[] { };

            using (MemoryStream stream = new MemoryStream(Serialize(message, SerializeError.NegativeMessageLength)))
            {
                var ex = await Assert.ThrowsExceptionAsync<ProtocolException>(async () =>
                {
                    Protocol protocol = new Protocol(stream);
                    byte[] receive = await protocol.ReceiveAsync();
                });
                Assert.AreEqual(ProtocolException.Error.NegativeMessageLength, ex.ErrorType);

            }
        }

        [TestMethod]
        public async Task ReceiveAsyncTooBigMessageLength()
        {
            byte[] message = new byte[] { };

            using (MemoryStream stream = new MemoryStream(Serialize(message, SerializeError.TooBigMessageLength)))
            {
                var ex = await Assert.ThrowsExceptionAsync<ProtocolException>(async () =>
                {
                    Protocol protocol = new Protocol(stream);
                    byte[] receive = await protocol.ReceiveAsync();
                });
                Assert.AreEqual(ProtocolException.Error.TooBigMessageLength, ex.ErrorType);

            }
        }
    }
}






