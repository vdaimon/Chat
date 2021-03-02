using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using ChatProtocol;

namespace ChatServer
{
    class ServerProgram
    {
        static int _port = 8005;
        static IPEndPoint _point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);


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

        static readonly object _lock = new object();
        static readonly List<ClientAttributes> _connections = new List<ClientAttributes>();

        static async Task SendToEveryone(ClientAttributes client, IGetBytes message)
        {
            List<ClientAttributes> connections;
            lock (_lock)
            {
                connections = new List<ClientAttributes>(_connections);
            }

            foreach (var el in connections)
            {
                if (el != client)
                    await el.ClientCommunicator.SendAsync(message);

            }

        }
        static async void HandleConnection(Socket socket)
        {
            ClientAttributes client = new ClientAttributes(socket, null);

            Console.WriteLine($"Client connected {socket.RemoteEndPoint}");
            try
            {
                while(socket.Connected)
                { 
                    var data = await client.ClientCommunicator.ReceiveAsync();


                    if (data.GetType() != typeof(AuthorizationMessage) && client.ClientName == null)
                        return;

                    switch (data)
                    {
                        case AuthorizationMessage am:
                            {
                                string name = am.UserName;
                                if (name == null)
                                    return;
                                client.ClientName = name;
                                lock (_lock)
                                {
                                    _connections.Add(client);
                                }

                                Console.WriteLine(DateTime.Now.ToShortTimeString() + socket.RemoteEndPoint + $": client {name} connected");

                                await SendToEveryone(client, new ConnectionNotificationMessage(name));
                                break;
                            }

                        case TextMessage tm:
                            {
                                string text = tm.Text;
                                if (text == null)
                                    return;
                                Console.WriteLine(DateTime.Now.ToShortTimeString() + $": {client.ClientName}: {text}");
                                await SendToEveryone(client, new TextMessage($"{client.ClientName}: {text}"));
                                break;
                            }

                        case RequestConnectionListMessage rcm:
                            {
                                string name = rcm.UserName;
                                if (name == null)
                                    return;
                                Console.WriteLine(DateTime.Now.ToShortTimeString() + $": {client.ClientName}: connection list request");

                                List<ClientAttributes> connections;
                                lock (_lock)
                                {
                                    connections = new List<ClientAttributes>(_connections);
                                }

                                List<string> connectionList = new List<string>();

                                foreach (var el in connections)
                                {
                                    connectionList.Add(el.ClientName);
                                }
                                await client.ClientCommunicator.SendAsync(new ConnectionListMessage(connectionList));
                                break;
                            }
                        case DisconnectionNotificationMessage dm:
                            {
                                string name = dm.UserName;
                                if (name == null)
                                    return;
                                await SendToEveryone(client, dm);
                                break;
                            }
                    }
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.ConnectionAborted && ex.SocketErrorCode != SocketError.ConnectionReset)
                    throw;
            }
            finally
            {
                lock (_lock)
                {
                    if (_connections.Contains(client))
                        _connections.Remove(client);
                }

                Console.WriteLine($"Client disconnected {socket.RemoteEndPoint}");
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        static void Main(string[] args)
        {
            var listener = new Listener(IPAddress.Parse("127.0.0.1"), _port);
            listener.Start(HandleConnection);

            Console.ReadLine();
            listener.Stop();

        }
    }
}
