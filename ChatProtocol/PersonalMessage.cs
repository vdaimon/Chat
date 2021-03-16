using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class PersonalMessage : MessageBase
    {
        public string ReceiverName { get; }
        public string SenderName { get; }
        public string Text { get; }

        public PersonalMessage(string senderName, string receiverName, string message, Guid transactionId)
            : base(Communicator.MessageType.Personal, transactionId)
        {
            ReceiverName = receiverName;
            SenderName = senderName;
            Text = message;
        }

        public PersonalMessage(Stream stream)
            : base(stream, Communicator.MessageType.Personal)
        {
                SenderName = stream.ReadString();
                ReceiverName = stream.ReadString();
                Text = stream.ReadString();
        }
        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
            stream.WriteString(SenderName);
            stream.WriteString(ReceiverName);
            stream.WriteString(Text);
        }
    }
}
