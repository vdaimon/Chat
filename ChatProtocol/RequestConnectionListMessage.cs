using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class RequestConnectionListMessage : UserNameMessage
    {
        public override Communicator.MessageType MessageType => Communicator.MessageType.RequestConnectionList;

        public RequestConnectionListMessage(byte[] userName)
            :base(userName)
        {

        }
        public RequestConnectionListMessage(string userName)
            : base(userName)
        {

        }

    }
}
