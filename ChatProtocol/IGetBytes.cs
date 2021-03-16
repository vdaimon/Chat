using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ChatProtocol.Communicator;

namespace ChatProtocol
{
    public interface IGetBytes
    {
        MessageType MessageType { get; }
        void ToStream(Stream stream);
    }
}
