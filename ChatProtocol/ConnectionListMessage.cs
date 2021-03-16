using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ConnectionListMessage : MessageBase
    {
        public List<string> UserNames { get; }

        public ConnectionListMessage(Stream stream)
            : base(stream, Communicator.MessageType.ConnectionList)
        {
                int listLenght = stream.ReadInt32();

                UserNames = new List<string>();

                for (;listLenght>0;listLenght--)
                {
                   UserNames.Add(stream.ReadString());
                }
        }

        public ConnectionListMessage(List<string> userNames, Guid transactionId)
            : base(Communicator.MessageType.ConnectionList, transactionId)
        {
            UserNames = userNames;
        }


        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
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
