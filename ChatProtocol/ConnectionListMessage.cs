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

        public ConnectionListMessage(MemoryStream packet)
        {
                int listLenght = packet.ReadInt32();

                UserNames = new List<string>();

                for (;listLenght>0;listLenght--)
                {
                   UserNames.Add(packet.ReadString());
                }
        }

        public ConnectionListMessage(List<string> userNames)
        {
            UserNames = userNames;
        }


        public void GetBytes(MemoryStream stream)
        {
                stream.WriteInt32(UserNames.Count);

                foreach(var el in UserNames)
                {
                    stream.WriteString(el);
                }
        }
    }

    // ConnectionListMessage message = new ConnectionListMessage(userNames);
    // socket.Send message.GetBytes();
}
