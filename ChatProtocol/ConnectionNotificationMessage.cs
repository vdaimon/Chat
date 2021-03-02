using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatProtocol.Communicator;

namespace ChatProtocol
{
    public class ConnectionNotificationMessage : UserNameMessage
    {
        public override MessageType MessageType => MessageType.ConnectionNotification;

        public ConnectionNotificationMessage(byte[] userName)
            :base(userName)
        {

        }

        public ConnectionNotificationMessage(string userName)
            : base(userName)
        {

        }
    }
}
