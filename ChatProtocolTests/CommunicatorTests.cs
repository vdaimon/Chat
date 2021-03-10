using ChatProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Styx.Crc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatProtocol.Communicator;

namespace ChatProtocolTests
{
    [TestClass]
    public class CommunicatorTests
    {

        private byte[] Serializer(IGetBytes message)
        {

            MemoryStream packet = new MemoryStream();
            using (packet)
            {
                packet.WriteByte((byte)message.MessageType);
                message.GetBytes(packet);
            }
            var data = packet.ToArray();
            var dataLen = BitConverter.GetBytes(data.Length);
            var buffer = new byte[] { };
            return buffer.Concat(Encoding.UTF8.GetBytes("BEGIN"))
                  .Concat(dataLen)
                  .Concat(BitConverter.GetBytes(Crc32.ComputeCrc(dataLen)))
                  .Concat(data)
                  .Concat(BitConverter.GetBytes(Crc32.ComputeCrc(data)))
                  .ToArray();
        }

        private async Task<T> ReceiveAsyncObject<T>(IGetBytes message) where T : class
        {
            using (MemoryStream stream = new MemoryStream(Serializer(message)))
            {
                var communicator = new Communicator(new Protocol(stream));
                var res = await communicator.ReceiveAsync() as T;

                Assert.IsNotNull(res);
                return res;
            }
        }

        [TestMethod]
        public async Task ReceiveAsyncAuthorization()
        {
            var data = new AuthorizationMessage("Dmk");
            var res = await ReceiveAsyncObject<AuthorizationMessage>(data);
            Assert.AreEqual(data.UserName, res.UserName);
        }

        [TestMethod]
        public async Task ReceiveAsyncText()
        {
            var data = new TextMessage("Hello, world!","Dmk");
            var res = await ReceiveAsyncObject<TextMessage>(data);
            Assert.AreEqual(data.Text, res.Text);
        }

        [TestMethod]
        public async Task ReceiveAsyncConnectionNotification()
        {
            var data = new ConnectionNotificationMessage ("Dmk");
            var res = await ReceiveAsyncObject<ConnectionNotificationMessage>(data);
            Assert.AreEqual(data.UserName, res.UserName);
        }

        [TestMethod]
        public async Task ReceiveAsyncDisconnectionNotification()
        {
            var data = new DisconnectionNotificationMessage("Dmk");
            var res = await ReceiveAsyncObject<DisconnectionNotificationMessage>(data);
            Assert.AreEqual(data.UserName, res.UserName);
        }

        [TestMethod]
        public async Task ReceiveAsyncConnectionList()
        {

            var data = new ConnectionListMessage(new List<string>{"Say", "Dmk"});
            var res = await ReceiveAsyncObject<ConnectionListMessage>(data);
            CollectionAssert.AreEqual(data.UserNames, res.UserNames);
        }

        [TestMethod]
        public async Task ReceiveAsyncRequestConnectionList()
        {
            var data = new RequestConnectionListMessage("Dmk");
            var res = await ReceiveAsyncObject<RequestConnectionListMessage>(data);
            Assert.AreEqual(data.UserName, res.UserName);
        }

        [TestMethod]
        public async Task ReceiveAsyncServerStopNotification()
        {
            await ReceiveAsyncObject<ServerStopNotificationMessage>(new ServerStopNotificationMessage());
        }

        private async Task SendAsyncObject(IGetBytes data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var communicator = new Communicator(new Protocol(stream));
                await communicator.SendAsync(data);

                CollectionAssert.AreEqual(Serializer(data), stream.ToArray());
            }
        }

        [TestMethod]
        public async Task SendAsyncAuthorization()
        {
            await SendAsyncObject(new AuthorizationMessage("Dmk"));
        }

        [TestMethod]
        public async Task SendAsyncText()
        {
            await SendAsyncObject(new TextMessage("Hello, world!", "Dmk"));
        }

        [TestMethod]
        public async Task SendAsyncConnectionNotification()
        {
            await SendAsyncObject(new ConnectionNotificationMessage("Dmk"));
        }

        [TestMethod]
        public async Task SendAsyncDisconnectionNotification()
        {
            await SendAsyncObject(new DisconnectionNotificationMessage("Dmk"));
        }

        [TestMethod]
        public async Task SendAsyncConnectionList()
        {
            await SendAsyncObject(new ConnectionListMessage(new List<string> { "Say", "Dmk" }));
        }

        [TestMethod]
        public async Task SendAsyncRequestConnectionList()
        {
            await SendAsyncObject(new RequestConnectionListMessage("Dmk"));
        }

        [TestMethod]
        public async Task SendAsyncServerStopNotification()
        {
            await SendAsyncObject(new ServerStopNotificationMessage());
        }

        class InvalidMessage : IGetBytes
        {
            public MessageType MessageType => (MessageType)0x58;

            public void GetBytes(MemoryStream stream)
            {
                stream.WriteString("Dmk");
            }
        }


        [TestMethod]
        public async Task SendAsyncInvalidMessage()
        {

            using (MemoryStream stream = new MemoryStream())
            {
                var communicator = new Communicator(new Protocol(stream));

                await Assert.ThrowsExceptionAsync<CommunicatorException>(async () =>
                {
                    await communicator.SendAsync(new InvalidMessage());
                });
            }
        }

        [TestMethod]
        public async Task ReceiveAsyncInvalidMessage()
        {
            var data = new InvalidMessage();

            using (MemoryStream stream = new MemoryStream(Serializer(data)))
            {
                var communicator = new Communicator(new Protocol(stream));

                await Assert.ThrowsExceptionAsync<CommunicatorException>(async () =>
                {
                    await communicator.ReceiveAsync();
                });
            }
        }
    }
}
