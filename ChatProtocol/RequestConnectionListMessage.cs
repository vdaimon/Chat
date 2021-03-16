using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class RequestConnectionListMessage : UserNameMessage
    {
        public RequestConnectionListMessage(Stream stream)
            :base(stream, Communicator.MessageType.RequestConnectionList)
        {

        }
        public RequestConnectionListMessage(string userName, Guid transactionId)
            : base(userName, transactionId, Communicator.MessageType.RequestConnectionList)
        {

        }

    }
}
