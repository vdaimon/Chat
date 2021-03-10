using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatProtocol.Communicator;

namespace ChatProtocol
{
    public class ConnectionNotificationMessage : UserNameMessage
    {
        public override MessageType MessageType => MessageType.ConnectionNotification;

        public ConnectionNotificationMessage(MemoryStream packet)
            :base(packet)
        {

        }

        public ConnectionNotificationMessage(string userName)
            : base(userName)
        {

        }
    }
}
