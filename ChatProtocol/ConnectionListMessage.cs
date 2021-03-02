using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ConnectionListMessage : IGetBytes
    {
        public List<string> UserNames { get; }

        public Communicator.MessageType MessageType => Communicator.MessageType.ConnectionList;

        public ConnectionListMessage(byte[] message)
        {

            using (Stream stream = new MemoryStream(message))
            {
                int listLenght = stream.ReadInt32();

                UserNames = new List<string>();

                for (;listLenght>0;listLenght--)
                {
                   UserNames.Add(stream.ReadString());
                }

            }
        }

        public ConnectionListMessage(List<string> userNames)
        {
            UserNames = userNames;
        }


        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream ())
            {
                stream.WriteInt32(UserNames.Count);

                foreach(var el in UserNames)
                {
                    stream.WriteString(el);
                }

                return stream.ToArray();   
            }

        }
    }

    // ConnectionListMessage message = new ConnectionListMessage(userNames);
    // socket.Send message.GetBytes();
}
