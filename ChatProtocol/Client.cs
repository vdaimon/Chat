using ChatProtocol;
using Styx.Crc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class Client
    {
        private IPEndPoint _endPoint;
        private Socket _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Communicator _communicator;
        public string Name { get; }

        public delegate void TextMessageHandler(string message);
        public event TextMessageHandler TextMessageReceived;

        public delegate void ConnectionNotificationMessageHandler(string message);
        public event ConnectionNotificationMessageHandler ConnectionNotificationMessageReceived;

        public delegate void DisconnectionNotificationMessageHandler(string message);
        public event DisconnectionNotificationMessageHandler DisconnectionNotificationMessageReceived;

        public delegate void ConnectionListMessageHandler(string message);
        public event ConnectionListMessageHandler ConnectionListMessageReceived;

        public delegate void ServerStopNotificationMessageHandler(string message);
        public event ServerStopNotificationMessageHandler ServerStopNotificationMessageReceived;

        public Client (IPAddress addr, int port, string name)
        {
            _endPoint = new IPEndPoint(addr, port);
            Name = name;
        }
        
        public async void ConnectAsync ()
        {
            await _client.ConnectAsync(_endPoint);

            _communicator = new Communicator(new Protocol(new NetworkStream(_client)));

            await _communicator.SendAsync(new AuthorizationMessage(Name));


            while (_client.Connected)
            {
                var data = await _communicator.ReceiveAsync();

                switch(data)
                {
                    case TextMessage tm:
                        {
                            TextMessageReceived(tm.Text);
                            break;
                        }
                    case ConnectionNotificationMessage cm:
                        {
                            ConnectionNotificationMessageReceived($"{cm.UserName} connected");
                            break;
                        }
                    case DisconnectionNotificationMessage dm:
                        {
                            DisconnectionNotificationMessageReceived($"{dm.UserName} disconnected");
                            break;
                        }
                    case ConnectionListMessage clm:
                        {
                            foreach (var el in clm.UserNames)
                                ConnectionListMessageReceived(el);
                            break;
                        }
                    case ServerStopNotificationMessage ssm:
                        {
                            ServerStopNotificationMessageReceived("Stopping the server");
                            break;
                        }
                    
                }
            }
        }

        public async Task SendAsync (IGetBytes message)
        {
             await _communicator.SendAsync(message);
        }

        public async Task Disconnect ()
        {
            await SendAsync(new DisconnectionNotificationMessage(Name));
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
    }
}
