using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ServerStopNotificationMessage : IGetBytes
    {
        public Communicator.MessageType MessageType => Communicator.MessageType.ServerStopNotification;

        public ServerStopNotificationMessage(MemoryStream packet = null)
        {

        }

        public void GetBytes(MemoryStream stream)
        {
            return;
        }
    }
}
