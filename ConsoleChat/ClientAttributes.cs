using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class ClientAttributes
    {
        public Protocol ClientProtocol { get; }
        public string ClientName { get; set; }

        public Communicator ClientCommunicator { get; }


        public ClientAttributes(Socket socket, string name)
        {
            ClientName = name;
            ClientProtocol = new Protocol(new NetworkStream(socket));
            ClientCommunicator = new Communicator(ClientProtocol);
        }
    }
}
