using ChatProtocol;
using Styx.Crc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
        public event EventHandler<object> ServerStopNotificationMessageReceived;
        public event EventHandler<object> PersonalMessageReceived;
        public event EventHandler<object> ServerStopped;

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
                ListenAsync();
            };

            var data = (ServerResponse) await Communicate(new AuthorizationMessage(Name, Guid.NewGuid()));

            return data.Response == ServerResponse.ResponseType.AuthorizationSuccessfully;
        }

        private readonly Dictionary<Guid, TaskCompletionSource<MessageBase>> _executingTasks = new Dictionary<Guid, TaskCompletionSource<MessageBase>>();

        public async Task<object> Communicate(MessageBase data)
        {
            var tcs = new TaskCompletionSource<MessageBase>();

            lock(_executingTasks)
                _executingTasks.Add(data.TransactionId, tcs);

            await _communicator.SendAsync(data);
            return await tcs.Task;
        }

        private TaskCompletionSource<MessageBase> GetTaskCompletetionSourceById(Guid transactionId)
        {
            lock (_executingTasks)
            {
                if (!_executingTasks.ContainsKey(transactionId))
                    return null;

                var tcs = _executingTasks[transactionId];
                _executingTasks.Remove(transactionId);

                return tcs;
            }
        }

        private void DataHandler(object state)
        {
            if (!(state is MessageBase data))
                return;

            var tcs = GetTaskCompletetionSourceById(data.TransactionId);

            if (tcs != null)
            {
                tcs.SetResult(data);
                return;
            }

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
                case ServerStopNotificationMessage ssm:
                    {
                        if (ServerStopNotificationMessageReceived != null)
                            ServerStopNotificationMessageReceived(this, ssm);
                        break;
                    }
                case PersonalMessage pm:
                    {
                        if (PersonalMessageReceived != null)
                            PersonalMessageReceived(this, pm);
                        break;
                    }
            }
        }

        private async void ListenAsync()
        {
            try
            {
                while (_client.Connected)
                {
                    var data = await _communicator.ReceiveAsync();
                    ThreadPool.QueueUserWorkItem(DataHandler, data);
                }
            }
            catch (System.IO.IOException ex)
            {
                if (ex.GetType() == typeof (System.IO.IOException))
                    if (ServerStopped != null)
                        ServerStopped(this, null);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SendTextMessageAsync(string message)
        {
            var res = (ServerResponse)await Communicate(new TextMessage(message, Name, Guid.NewGuid()));

            if (res.Response == ServerResponse.ResponseType.MessageSendSuccessfully)
                return;

            throw new Exception($"Unexpected result {res.Response}");
        }

        public async Task<List<string>> GetConnectionListAsync()
        {
            var res = await Communicate(new RequestConnectionListMessage(Name, Guid.NewGuid()));

            if (res is ConnectionListMessage clm)
                return clm.UserNames;

            if (res is ServerResponse sr)
                throw new Exception($"Unexpected result {sr.Response}");

            throw new Exception($"Unexpected response type {res.GetType()}");
        }

        public async Task DisconnectAsync ()
        {
            if (!_client.Connected)
                return;
            await _communicator.SendAsync(new DisconnectionNotificationMessage(Name, Guid.NewGuid()));
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
        }

        public async Task SendPersonallyMessage(string receiverName, string message)
        {
            var res = (ServerResponse)await Communicate(new PersonalMessage(Name, receiverName, message, Guid.NewGuid()));

            if (res.Response == ServerResponse.ResponseType.MessageSendSuccessfully)
                return;

            if (res.Response == ServerResponse.ResponseType.RecipientNotFound)
                throw new Exception("Recipient not found");

            throw new Exception($"Unexpected result {res.Response}");
        }
    }
}
