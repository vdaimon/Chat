using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChatProtocol
{
    public class TextMessage : MessageBase
    {
        public string Text { get; }
        public string UserName { get; }

        public TextMessage(Stream stream)
            : base(stream, Communicator.MessageType.Text)
        {
            UserName = stream.ReadString();
            Text = stream.ReadString();
        }

        public TextMessage(string message, string userName, Guid transactionId)
            : base(Communicator.MessageType.Text, transactionId)
        {
            Text = message;
            UserName = userName;
        }

        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
            stream.WriteString(UserName);
            stream.WriteString(Text);
        }
    }
}
