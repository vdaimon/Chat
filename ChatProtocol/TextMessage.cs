using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatProtocol.Communicator;

namespace ChatProtocol
{
    public class TextMessage : IGetBytes
    {
        public string Text { get; }
        MessageType IGetBytes.MessageType => MessageType.Text;
        public string UserName { get; }

        public TextMessage(MemoryStream packet)
        {
            UserName = packet.ReadString();
            Text = packet.ReadString();
        }

        public TextMessage(string message, string userName)
        {
            Text = message;
            UserName = userName;
        }

        public void GetBytes(MemoryStream stream)
        {
            stream.WriteString(UserName);
            stream.WriteString(Text);
        }
    }
}
