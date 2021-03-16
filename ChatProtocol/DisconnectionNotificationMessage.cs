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
        public DisconnectionNotificationMessage(Stream stream)
            :base(stream, Communicator.MessageType.DisconnectionNotification)
        {

        }

        public DisconnectionNotificationMessage(string userName, Guid transactionId)
            : base(userName, transactionId, Communicator.MessageType.DisconnectionNotification)
        {

        }
    }
}
