﻿using ChatProtocol;
using Styx.Crc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    class Client
    {
        private IPEndPoint _endPoint;
        private Socket _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Communicator _communicator;
        public string Name { get; }

        public delegate void MessageHandler(string message);
        public event MessageHandler MessageReceived;

        public Client (IPAddress addr, int port, string name)
        {
            _endPoint = new IPEndPoint(addr, port);
            Name = name;
        }
        
        public async Task Connect ()
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
                            MessageReceived(tm.Text);
                            break;
                        }
                    case ConnectionNotificationMessage cm:
                        {
                            MessageReceived($"{cm.UserName} connected");
                            break;
                        }
                    case DisconnectionNotificationMessage dm:
                        {
                            MessageReceived($"{dm.UserName} disconnected");
                            break;
                        }
                    case ConnectionListMessage clm:
                        {
                            foreach (var el in clm.UserNames)
                                MessageReceived(el);
                            break;
                        }
                    case ServerStopNotificationMessage ssm:
                        {
                            MessageReceived("Stopping the server");
                            break;
                        }
                    
                }
            }
        }

        public async Task SendAsync (IGetBytes message)
        {
             await _communicator.SendAsync(message);
        }

        public async void Disconnect ()
        {
            await SendAsync(new DisconnectionNotificationMessage(Name));
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }

        


    }
}