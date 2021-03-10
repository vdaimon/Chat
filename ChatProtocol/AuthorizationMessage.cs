using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class AuthorizationMessage : UserNameMessage
    {
        public override Communicator.MessageType MessageType => Communicator.MessageType.Autorization;

        public AuthorizationMessage(MemoryStream packet)
            : base(packet)
        {
            
        }

        public AuthorizationMessage(string userName)
            : base(userName)
        {

        }

    }
}
