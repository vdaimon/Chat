using System;
using System.Collections.Generic;
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

        public TextMessage(byte[] message)
        {
            Text = Encoding.UTF8.GetString(message);
        }

        public TextMessage(string message)
        {
            Text = message;
        }
        byte[] IGetBytes.GetBytes()
        {
            return Encoding.UTF8.GetBytes(Text);
        }
    }
}
