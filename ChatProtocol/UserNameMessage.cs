using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class UserNameMessage : MessageBase
    {
        public string UserName { get; }

        protected UserNameMessage(Stream stream, Communicator.MessageType msgType)
            : base(stream, msgType)
        {
            UserName = stream.ReadString();
        }

        protected UserNameMessage(string userName, Guid transactionId, Communicator.MessageType msgType)
            : base(msgType, transactionId)
        {
            UserName = userName;
        }

        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
            stream.WriteString(UserName);
        }
    }
}
