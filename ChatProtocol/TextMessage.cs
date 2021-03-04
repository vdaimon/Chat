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

        public TextMessage(byte[] message)
        {
            using(var stream = new MemoryStream(message))
            {
                UserName = stream.ReadString();
                Text = stream.ReadString();
            }
        }

        public TextMessage(string message, string userName)
        {
            Text = message;
            UserName = userName;
        }

        byte[] IGetBytes.GetBytes()
        {
            using (var stream = new MemoryStream())
            {
                stream.WriteString(UserName);
                stream.WriteString(Text);
                return stream.ToArray();
            }
        }
    }
}
