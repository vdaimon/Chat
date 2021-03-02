using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class UserNameMessage : IGetBytes
    {
        public string UserName { get; }

        public virtual Communicator.MessageType MessageType => throw new NotImplementedException();

        protected UserNameMessage(byte[] message)
        {
            UserName = Encoding.UTF8.GetString(message);
        }

        protected UserNameMessage(string userName)
        {
            UserName = userName;
        }

        public byte[] GetBytes()
        {
            return Encoding.UTF8.GetBytes(UserName);
        }
    }
}
