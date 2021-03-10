using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class DisconnectionNotificationMessage : UserNameMessage
    {
        public override Communicator.MessageType MessageType => Communicator.MessageType.DisconnectionNotification;

        public DisconnectionNotificationMessage(MemoryStream packet)
            :base(packet)
        {

        }

        public DisconnectionNotificationMessage(string userName)
            : base(userName)
        {

        }
    }
}
