using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class CommunicatorException : Exception
    {
        public CommunicatorException()
            :base("Invalid message type")
        {

        }
    }
}
