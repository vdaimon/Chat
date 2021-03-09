using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFClient
{
    public class ChatWindowDataContext : INPC
    {
        private bool _EnableSendButtonFlag;
        private string _message;
        private string _receiver;
        public string Message { get => _message; set { Set(ref _message, value); if (_message != null) EnableSendButtonFlag = true; } }
        public bool EnableSendButtonFlag { get => _EnableSendButtonFlag; set => Set(ref _EnableSendButtonFlag, value); }
        public string UserName { get; set; }
        public string Receiver { get => _receiver; set { Set(ref _receiver, value); } }
        public Client Client { get; set; }
        public ObservableCollection<string> ConnectionList { get; } = new ObservableCollection<string>();
        public ObservableCollection<TextMessage> Messages { get; } = new ObservableCollection<TextMessage>();

        public ChatWindowDataContext()
        {
            Client = new Client(IPAddress.Loopback, 8005);
        }

        public async void StartClient(Window owner)
        {
            string text = "Enter your username";
            do
            {
                AuthorizationWindow dlg = new AuthorizationWindow();
                dlg.dataContext.Text = text;
                dlg.Owner = owner;
                dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dlg.ShowDialog();
                UserName = dlg.dataContext.UserName;
                text = "The name is already in use. \nEnter your username";
            } while (!await Client.ConnectAsync(UserName));

            ConnectingEventHandlers();

            await Client.SendRequestConnectionListAsync();
        }

        private void ConnectingEventHandlers()
        {
            Client.ConnectionListMessageReceived += (_, clm) =>
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

            Client.ServerStopNotificationMessageReceived += (_, ssn) =>
            {

            };
            Client.ClientToClientMessageReceived += (_, ctcm) =>
            {
                var res = (ClientToClientTextMessage)ctcm;
                Messages.Add(new TextMessage(res.Text, $"Personally from {res.SenderName}"));
            };
        }

        public async Task DisconnectAsync()
        {
           await Client.DisconnectAsync();
        }
        public async void SendTextAsync()
        {

            if (Receiver != null)
            {
                Messages.Add(new TextMessage(Message, $"You to {Receiver}"));
                await Client.SendPersonallyMessage(Receiver, Message);
            }
            else
            {
                Messages.Add(new TextMessage(Message, "You"));
                await Client.SendTextMessageAsync(Message);
            }
            Message = null;
            EnableSendButtonFlag = false;
            Receiver = null;
        }

    }

    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private ChatWindowDataContext _dataContext;
        public ChatWindow()
        {
            InitializeComponent();

            _dataContext = new ChatWindowDataContext();
            DataContext = _dataContext;

            Loaded += (s, e) => _dataContext.StartClient(this);
            Closing += ClosingHandler;
        }

        private async void ClosingHandler(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            await _dataContext.DisconnectAsync();
            Closing -= ClosingHandler;
            Close();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SendTextAsync();
        }

        
    }
}

