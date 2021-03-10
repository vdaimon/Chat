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
        public override Communicator.MessageType MessageType => Communicator.MessageType.RequestConnectionList;

        public RequestConnectionListMessage(MemoryStream packet)
            :base(packet)
        {

        }
        public RequestConnectionListMessage(string userName)
            : base(userName)
        {

        }

    }
}
