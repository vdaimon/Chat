using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class UserNameMessage : IGetBytes
    {
        public string UserName { get; }

        public virtual Communicator.MessageType MessageType => throw new NotImplementedException();

        protected UserNameMessage(MemoryStream packet)
        {
            UserName = packet.ReadString();
        }

        protected UserNameMessage(string userName)
        {
            UserName = userName;
        }

        public void GetBytes(MemoryStream stream)
        {
            stream.WriteString(UserName);
        }
    }
}
