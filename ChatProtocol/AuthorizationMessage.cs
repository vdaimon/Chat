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

        public AuthorizationMessage(Stream stream)
            : base(stream, Communicator.MessageType.Autorization)
        {
            
        }

        public AuthorizationMessage(string userName, Guid transactionId)
            : base(userName, transactionId, Communicator.MessageType.Autorization)
        {

        }

    }
}
