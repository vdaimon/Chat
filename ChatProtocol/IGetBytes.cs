using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatProtocol.Communicator;

namespace ChatProtocol
{
    public interface IGetBytes
    {
        MessageType MessageType { get; }
        byte[] GetBytes();
    }
}
