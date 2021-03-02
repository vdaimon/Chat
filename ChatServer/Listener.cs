using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    class Listener
    {
        private Socket _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Listener(IPAddress addr, int tcpPort)
        {
            IPEndPoint point = new IPEndPoint(addr, tcpPort);
            _listener.Bind(point);
        }

        public delegate void ConnectionHandler(Socket socket);

        public async void Start(ConnectionHandler handler)
        {
            _listener.Listen(1);
            while (true)
            {
                handler(await _listener.AcceptAsync());
            }
        }

        public void Stop()
        {
            _listener.Close();
        }
    }
}
