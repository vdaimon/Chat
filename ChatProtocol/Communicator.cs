using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class Communicator
    {
        private Protocol _protocol;
        public enum MessageType : byte
        {
            Autorization,
            Text,
            ConnectionNotification,
            DisconnectionNotification,
            ConnectionList,
            RequestConnectionList,
            ServerStopNotification,
            SuccessfulAuthorizationNotification,
            Personal,
            ServerException,
            ServerResponse,
        }

        public Communicator(Protocol protocol)
        {
            _protocol = protocol;
        }

        public async Task<MessageBase> ReceiveAsync()
        {
            var data = await _protocol.ReceiveAsync();


            using (MemoryStream packet = new MemoryStream(data))
            {

                var msgType = (MessageType)packet.ReadByte();

                switch (msgType)
                {
                    case MessageType.Autorization:
                        return new AuthorizationMessage(packet);

                    case MessageType.Text:
                        return new TextMessage(packet);

                    case MessageType.ConnectionNotification:
                        return new ConnectionNotificationMessage(packet);

                    case MessageType.DisconnectionNotification:
                        return new DisconnectionNotificationMessage(packet);

                    case MessageType.ConnectionList:
                        return new ConnectionListMessage(packet);

                    case MessageType.RequestConnectionList:
                        return new RequestConnectionListMessage(packet);

                    case MessageType.ServerStopNotification:
                        return new ServerStopNotificationMessage(packet);

                    case MessageType.SuccessfulAuthorizationNotification:
                        return new SuccessfulAuthorizationNotificationMessage(packet);

                    case MessageType.Personal:
                        return new PersonalMessage(packet);

                    case MessageType.ServerException:
                        return new ServerExceptionMessage(packet);

                    case MessageType.ServerResponse:
                        return new ServerResponse(packet);

                    default:
                        throw new CommunicatorException();

                }
            }
        }

        public async Task SendAsync(IGetBytes message)
        {
            if (!Enum.IsDefined(typeof(MessageType), message.MessageType))
                throw new CommunicatorException();
            using (MemoryStream stream = new MemoryStream()) 
            {
                stream.WriteByte((byte)message.MessageType);
                message.ToStream(stream);
                await _protocol.SendAsync(stream.ToArray());
            }
        }
    }
}
