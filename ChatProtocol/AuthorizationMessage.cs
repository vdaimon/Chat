using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class AuthorizationMessage : UserNameMessage
    {
        public override Communicator.MessageType MessageType => Communicator.MessageType.Autorization;

        public AuthorizationMessage(byte[] message)
            : base(message)
        {
            
        }

        public AuthorizationMessage(string userName)
            : base(userName)
        {

        }

    }
}
