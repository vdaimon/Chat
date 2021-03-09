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

        public ClientToClientTextMessage(byte[] packet)
        {
            using (MemoryStream stream = new MemoryStream(packet))
            {
                SenderName = stream.ReadString();
                ReceiverName = stream.ReadString();
                Text = stream.ReadString();
            }
        }
        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.WriteString(SenderName);
                stream.WriteString(ReceiverName);
                stream.WriteString(Text);
                return stream.ToArray();
            }
        }
    }
}
