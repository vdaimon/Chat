using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ChatProtocol;
using System.Linq;
using static ChatProtocol.ServerExceptionMessage;
using static ChatProtocol.ServerResponse;

namespace ChatServer
{
    class ServerProgram
    {

        static readonly object _lock = new object();
        static readonly List<ClientAttributes> _connections = new List<ClientAttributes>();

        static async Task SendToEveryone(IGetBytes message, ClientAttributes excludedClient = null)
        {
            List<ClientAttributes> connections;
            lock (_lock)
            {
                connections = new List<ClientAttributes>(_connections);
            }

            foreach (var el in connections)
            {
                if (el != excludedClient)
                    await el.ClientCommunicator.SendAsync(message);
            }

        }
        static async Task ProcessMessage(object data, ClientAttributes client)
        {
            if (data.GetType() != typeof(AuthorizationMessage) && client.ClientName == null)
                return;

            switch (data)
            {
                case AuthorizationMessage am:
                    {
                        string name = am.UserName;
                        if (name == null)
                            return;

                        int similarNames = 0;
                        lock (_lock)
                        {
                            similarNames = _connections.Count((x) => x.ClientName == name);
 
                        }
                        if (similarNames!=0)
                        {
                            await client.ClientCommunicator.SendAsync(new ServerResponse(ResponseType.NameAlreadyInUse, am.TransactionId));
                            return;
                        }

                        await client.ClientCommunicator.SendAsync(new ServerResponse(ResponseType.AuthorizationSuccessfully, am.TransactionId));

                        client.ClientName = name;
                        lock (_lock)
                        {
                            _connections.Add(client);
                        }

                        Console.WriteLine(DateTime.Now.ToShortTimeString() + client.Socket.RemoteEndPoint + $": client {name} connected");

                        await SendToEveryone(new ConnectionNotificationMessage(name, Guid.NewGuid()), client);
                        break;
                    }

                case TextMessage tm:
                    {
                        string text = tm.Text;
                        if (text == null)
                        {
                            await client.ClientCommunicator.SendAsync(new ServerResponse(ResponseType.RecipientNotFound, tm.TransactionId));
                            return;
                        }
                        Console.WriteLine(DateTime.Now.ToShortTimeString() + $": {client.ClientName}: {text}");
                        await client.ClientCommunicator.SendAsync(new ServerResponse(ResponseType.MessageSendSuccessfully, tm.TransactionId));
                        await SendToEveryone(new TextMessage(text, client.ClientName, Guid.NewGuid()), client);
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
                        await client.ClientCommunicator.SendAsync(new ConnectionListMessage(connectionList, rcm.TransactionId));
                        break;
                    }
                case DisconnectionNotificationMessage dm:
                    {
                        string name = dm.UserName;
                        if (name == null)
                            return;
                        await SendToEveryone(dm , client);
                        break;
                    }
                case PersonalMessage pm:
                    {
                        ClientAttributes receiver;
                        lock (_lock)
                        {
                            receiver = _connections.FirstOrDefault((x) => x.ClientName == pm.ReceiverName);
                        }
                        if (receiver == null)
                        {
                            Console.WriteLine($"Failed to send personal message from {client.ClientName}");
                            await client.ClientCommunicator.SendAsync(new ServerResponse(ResponseType.RecipientNotFound, pm.TransactionId));
                            break;
                        }
                        await client.ClientCommunicator.SendAsync(new ServerResponse(ResponseType.MessageSendSuccessfully, pm.TransactionId));
                        await receiver.ClientCommunicator.SendAsync(pm);
                        Console.WriteLine($"from {pm.UserName} to {pm.ReceiverName}: {pm.Text}");
                        break;
                    }
            }
        }
        static async void HandleConnection(Socket socket)
        {
            ClientAttributes client = new ClientAttributes(socket, null);

            Console.WriteLine($"Client connected {socket.RemoteEndPoint}");
            try 
            { 
                while (socket.Connected)
                {
                    try
                    {
                        await ProcessMessage(await client.ClientCommunicator.ReceiveAsync(), client);
                    }
                    catch (ProtocolException ex)
                    {
                        if (ex.ErrorType == ProtocolException.Error.FailedToRead)
                            break;
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex);
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
            var listener = new Listener(IPAddress.Parse("127.0.0.1"), 8005);
            listener.Start(HandleConnection);

            var cmd = Console.ReadLine();
            if (cmd == "end")
            { 
                SendToEveryone(new ServerStopNotificationMessage()).Wait();
                listener.Stop();
            }
            

        }
    }
}