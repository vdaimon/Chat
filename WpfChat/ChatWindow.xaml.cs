using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFClient
{
    public class ChatWindowDataContext
    {
        public string UserName { get; }
        public Client Client { get; }
        public ObservableCollection<string> ConnectionList { get; } = new ObservableCollection<string>();
        public ObservableCollection<TextMessage> Messages { get; } = new ObservableCollection<TextMessage>();

        public ChatWindowDataContext(string userName)
        {
            UserName = userName;
            Client = new Client(IPAddress.Loopback, 8005, userName);
        }
        public async void ClientAction()
        {
            await Client.ConnectAsync();
            Client.Listen();
            Client.ConnectionListMessageReceived += ( _, clm) =>
            {
                var msg = (ConnectionListMessage)clm;

                foreach (var el in msg.UserNames)
                    ConnectionList.Add(el);
            };

            Client.ConnectionNotificationMessageReceived += (_, cnm) =>
            {
                var msg = (ConnectionNotificationMessage)cnm;
                ConnectionList.Add(msg.UserName);
            };

            Client.DisconnectionNotificationMessageReceived += (_, dnm) =>
            {
                var msg = (DisconnectionNotificationMessage)dnm;
                ConnectionList.Remove(msg.UserName);

            };

            Client.TextMessageReceived += (_, tm) =>
            {
                Messages.Add((TextMessage)tm);
            };

            var rclm = new RequestConnectionListMessage(Encoding.UTF8.GetBytes(Client.Name));
            await Client.SendAsync(rclm);

        }
    }

        /// <summary>
        /// Interaction logic for ChatWindow.xaml
        /// </summary>
        public partial class ChatWindow : Window
        {
            private ChatWindowDataContext _dataContext;
            public ChatWindow(string userName)
            {
                InitializeComponent();
                _dataContext = new ChatWindowDataContext(userName);
                _dataContext.ClientAction();
                DataContext = _dataContext;
            }
        }
    }

