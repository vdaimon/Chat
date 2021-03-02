using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ServerStopNotificationMessage : IGetBytes
    {
        public Communicator.MessageType MessageType => Communicator.MessageType.ServerStopNotification;

        public ServerStopNotificationMessage(byte[] data = null)
        {

        }

        public byte[] GetBytes()
        {
            return new byte[] { };
        }
    }
}
