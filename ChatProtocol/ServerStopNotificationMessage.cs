using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ServerStopNotificationMessage : MessageBase
    {
 
        public ServerStopNotificationMessage(Stream stream)
            : base(stream, Communicator.MessageType.ServerStopNotification)
        {

        }
        public ServerStopNotificationMessage()
            : base (Communicator.MessageType.ServerStopNotification, Guid.Empty)
        {

        }

        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
        }
    }
}
