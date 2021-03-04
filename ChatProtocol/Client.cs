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

        public event EventHandler<object> TextMessageReceived;
        public event EventHandler<object> ConnectionNotificationMessageReceived;
        public event EventHandler<object> DisconnectionNotificationMessageReceived;
        public event EventHandler<object> ConnectionListMessageReceived;
        public event EventHandler<object> ServerStopNotificationMessageReceived;

        public Client (IPAddress addr, int port, string name)
        {
            _endPoint = new IPEndPoint(addr, port);
            Name = name;
        }
        
        public async Task ConnectAsync ()
        {
            await _client.ConnectAsync(_endPoint);

            _communicator = new Communicator(new Protocol(new NetworkStream(_client)));

            await _communicator.SendAsync(new AuthorizationMessage(Name));


            //while (_client.Connected)
            //{
            //    var data = await _communicator.ReceiveAsync();
            //    switch (data)
            //    {
            //        case TextMessage tm:
            //            {
            //                TextMessageReceived(tm);
            //                break;
            //            }
            //        case ConnectionNotificationMessage cm:
            //            {
            //                ConnectionNotificationMessageReceived(cm);
            //                break;
            //            }
            //        case DisconnectionNotificationMessage dm:
            //            {
            //                DisconnectionNotificationMessageReceived($"{dm.UserName} disconnected");
            //                break;
            //            }
            //        case ConnectionListMessage clm:
            //            {
            //                foreach (var el in clm.UserNames)
            //                    ConnectionListMessageReceived(el);
            //                break;
            //            }
            //        case ServerStopNotificationMessage ssm:
            //            {
            //                ServerStopNotificationMessageReceived("Stopping the server");
            //                break;
            //            }

            //    }
            //}
        }

        public async void Listen()
        {
            while (_client.Connected)
            {
                var data = await _communicator.ReceiveAsync();
                switch (data)
                {
                    case TextMessage tm:
                        {
                            TextMessageReceived(this, tm);
                            break;
                        }
                    case ConnectionNotificationMessage cm:
                        {
                            ConnectionNotificationMessageReceived(this, cm);
                            break;
                        }
                    case DisconnectionNotificationMessage dm:
                        {
                            DisconnectionNotificationMessageReceived(this, dm);
                            break;
                        }
                    case ConnectionListMessage clm:
                        {
                            ConnectionListMessageReceived(this, clm);
                            break;
                        }
                    case ServerStopNotificationMessage ssm:
                        {
                            ServerStopNotificationMessageReceived(this, ssm);
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
