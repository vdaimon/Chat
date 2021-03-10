using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ClientToClientTextMessage : IGetBytes
    {
        public string ReceiverName { get; }
        public string SenderName { get; }
        public string Text { get; }
        public Communicator.MessageType MessageType => Communicator.MessageType.ClientToClientText;

        public ClientToClientTextMessage(string senderName, string receiverName, string message )
        {
            ReceiverName = receiverName;
            SenderName = senderName;
            Text = message;
        }

        public ClientToClientTextMessage(MemoryStream packet)
        {
                SenderName = packet.ReadString();
                ReceiverName = packet.ReadString();
                Text = packet.ReadString();
        }
        public void GetBytes(MemoryStream stream)
        {
            stream.WriteString(SenderName);
            stream.WriteString(ReceiverName);
            stream.WriteString(Text);
        }
    }
}
