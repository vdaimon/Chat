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
        public string Name { get; set; }

        public event EventHandler<object> TextMessageReceived;
        public event EventHandler<object> ConnectionNotificationMessageReceived;
        public event EventHandler<object> DisconnectionNotificationMessageReceived;
        public event EventHandler<object> ConnectionListMessageReceived;
        public event EventHandler<object> ServerStopNotificationMessageReceived;
        public event EventHandler<object> SuccessfulAuthorizationNotificationMessageReceived;
        public event EventHandler<object> ClientToClientMessageReceived;

        public Client (IPAddress addr, int port)
        {
            _endPoint = new IPEndPoint(addr, port);
        }


        public async Task<bool> ConnectAsync(string userName)
        {
            Name = userName;

            if (!_client.Connected)
            {
                await _client.ConnectAsync(_endPoint);
                _communicator = new Communicator(new Protocol(new NetworkStream(_client)));
            };

            await _communicator.SendAsync(new AuthorizationMessage(Name));
            var data = await _communicator.ReceiveAsync();
            var res = (SuccessfulAuthorizationNotificationMessage)data;
            if (res.IsSuccessful)
                ListenAsync();
            return res.IsSuccessful;

        }


        private async void ListenAsync()
        {
            try
            {
                while (_client.Connected)
                {
                    var data = await _communicator.ReceiveAsync();
                    switch (data)
                    {
                        case TextMessage tm:
                            {
                                if (TextMessageReceived != null)
                                    TextMessageReceived(this, tm);
                                break;
                            }
                        case ConnectionNotificationMessage cm:
                            {
                                if (ConnectionNotificationMessageReceived != null)
                                    ConnectionNotificationMessageReceived(this, cm);
                                break;
                            }
                        case DisconnectionNotificationMessage dm:
                            {
                                if (DisconnectionNotificationMessageReceived != null)
                                    DisconnectionNotificationMessageReceived(this, dm);
                                break;
                            }
                        case ConnectionListMessage clm:
                            {
                                if (ConnectionListMessageReceived != null)
                                    ConnectionListMessageReceived(this, clm);
                                break;
                            }
                        case ServerStopNotificationMessage ssm:
                            {
                                if (ServerStopNotificationMessageReceived != null)
                                    ServerStopNotificationMessageReceived(this, ssm);
                                break;
                            }
                        case SuccessfulAuthorizationNotificationMessage san:
                            {
                                if (SuccessfulAuthorizationNotificationMessageReceived != null)
                                    SuccessfulAuthorizationNotificationMessageReceived(this, san);
                                break;
                            }
                        case ClientToClientTextMessage ctc:
                            {
                                if (ClientToClientMessageReceived != null)
                                    ClientToClientMessageReceived(this, ctc);
                                break;
                            }
                    }
                }
           
            }
            catch (Exception)
            {
            }
        }

        public async Task SendTextMessageAsync(string message)
        {
            await SendAsync(new TextMessage(message, Name));
        }
        public async Task SendRequestConnectionListAsync()
        {
            await SendAsync(new RequestConnectionListMessage(Name));
        }

        private async Task SendAsync (IGetBytes message)
        {
             await _communicator.SendAsync(message);
        }

        public async Task DisconnectAsync ()
        {
            await SendAsync(new DisconnectionNotificationMessage(Name));
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }
        public async Task SendPersonallyMessage(string receiverName, string message)
        {
            await SendAsync(new ClientToClientTextMessage(Name, receiverName, message));
        }
    }
}
