using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChatProtocol
{
    public class ConnectionNotificationMessage : UserNameMessage
    {

        public ConnectionNotificationMessage(Stream stream)
            :base(stream, Communicator.MessageType.ConnectionNotification)
        {

        }

        public ConnectionNotificationMessage(string userName, Guid transactionId)
            : base(userName, transactionId, Communicator.MessageType.ConnectionNotification)
        {

        }
    }
}
