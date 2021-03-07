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
        }

        public Communicator(Protocol protocol)
        {
            _protocol = protocol;
        }

        public async Task<object> ReceiveAsync()
        {
            var data = await _protocol.ReceiveAsync();

            MessageType msgType = (MessageType)data[0];

            var message = data.Skip(1).ToArray();

            switch(msgType)
            {
                case MessageType.Autorization:
                    return new AuthorizationMessage(message);

                case MessageType.Text:
                    return new TextMessage(message);

                case MessageType.ConnectionNotification:
                    return new ConnectionNotificationMessage(message);

                case MessageType.DisconnectionNotification:
                    return new DisconnectionNotificationMessage(message);

                case MessageType.ConnectionList:
                    return new ConnectionListMessage(message);

                case MessageType.RequestConnectionList:
                    return new RequestConnectionListMessage(message);

                case MessageType.ServerStopNotification:
                    return new ServerStopNotificationMessage(message);

                default:
                    throw new CommunicatorException();

            }
        }

        public async Task SendAsync(IGetBytes message)
        {
            if (!Enum.IsDefined(typeof(MessageType), message.MessageType))
                throw new CommunicatorException();
            await _protocol.SendAsync(new byte[] { (byte)message.MessageType }.Concat(message.GetBytes()).ToArray());
        }

    }
}
