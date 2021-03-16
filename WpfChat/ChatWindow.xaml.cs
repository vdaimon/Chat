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
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;

namespace WPFClient
{
    public class ChatWindowDataContext : INPC
    {
        private readonly Dispatcher _dispatcher;
        private bool _EnableSendButtonFlag;
        private string _message;
        private string _receiver;
        public string Message { get => _message; set { Set(ref _message, value, () => EnableSendButtonFlag = !string.IsNullOrEmpty(value)); } }
        public bool EnableSendButtonFlag { get => _EnableSendButtonFlag; set => Set(ref _EnableSendButtonFlag, value); }
        public string UserName { get; set; }
        public string Receiver { get => _receiver; set { Set(ref _receiver, value); } }
        public Client Client { get; set; }
        public ObservableCollection<string> ConnectionList { get; } = new ObservableCollection<string>();
        public ObservableCollection<TextMessage> Messages { get; } = new ObservableCollection<TextMessage>();


        public ChatWindowDataContext()
        {
            _dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            Client = new Client(IPAddress.Loopback, 8005);
            Receiver = "all";
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
                Client = new Client(dlg.dataContext.IP, dlg.dataContext.Port);
                if (UserName == ""|| UserName == null)
                    owner.Close();
            } while (!await Client.ConnectAsync(UserName));

            ConnectingEventHandlers();

            foreach (var el in await Client.GetConnectionListAsync())
            {
                ConnectionList.Add(el);
            }
        }

        private void ConnectingEventHandlers()
        {
            Client.ConnectionNotificationMessageReceived += (_, cnm) =>
            {
                var msg = (ConnectionNotificationMessage)cnm;
                _dispatcher.Invoke(() => ConnectionList.Add(msg.UserName));
            };

            Client.DisconnectionNotificationMessageReceived += (_, dnm) =>
            {
                var msg = (DisconnectionNotificationMessage)dnm;
                _dispatcher.Invoke(() => ConnectionList.Remove(msg.UserName));
            };

            Client.TextMessageReceived += (_, tm) =>
            {
                _dispatcher.Invoke(() => Messages.Add((TextMessage)tm));
            };

            Client.ServerStopNotificationMessageReceived += (_, ssn) =>
            {
                _dispatcher.Invoke(() =>
                {
                    Messages.Add(new TextMessage("The server has stopped", "Server", Guid.NewGuid()));
                    ConnectionList.Clear();
                });
            };

            Client.PersonalMessageReceived += (_, pm) =>
            {
                var res = (PersonalMessage)pm;
                _dispatcher.Invoke(() => Messages.Add(new TextMessage(res.Text, $"Personally from {res.SenderName}", Guid.NewGuid())));
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
                Messages.Add(new TextMessage(Message, $"You to {Receiver}", Guid.NewGuid()));
                await Client.SendPersonallyMessage(Receiver, Message);
            }
            else
            {
                Messages.Add(new TextMessage(Message, "You", Guid.NewGuid()));
                await Client.SendTextMessageAsync(Message);
            }
            Message = null;
            EnableSendButtonFlag = false;
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

            MessageBox.Focus();
        }

        private void ClosingHandler(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Closing -= ClosingHandler;
            _dataContext.DisconnectAsync().ContinueWith(_ =>
            {
                Dispatcher.Invoke(() => Close());
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SendTextAsync();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Focus();
            if (e.Key == Key.Escape)
                _dataContext.Receiver = null;
        }

    }
}

