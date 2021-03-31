using ChatProtocol;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Data;
using System.Linq;

namespace WPFClient
{
    public class ChatWindowDataContext : INPC
    {
        private readonly Dispatcher _dispatcher;
        private bool _enableSendButtonFlag;
        private string _message;
        private UserListElement _receiver;
        public string Message { get => _message; set { Set(ref _message, value, () => EnableSendButtonFlag = !string.IsNullOrEmpty(value)); } }
        public bool EnableSendButtonFlag { get => _enableSendButtonFlag; set => Set(ref _enableSendButtonFlag, value); }
        public string UserName { get; set; }

        public UserListElement Receiver
        { 
            get => _receiver;
            set => Set(ref _receiver, value, () => OnRecevierChanged(value));
        }

        public Client Client { get; set; }
        public ObservableCollection<UserListElement> ConnectionList { get; } = new ObservableCollection<UserListElement>();
        public ObservableCollection<MessageBase> Messages { get; } = new ObservableCollection<MessageBase>();

        private ICollectionView _messageList;
        public ICollectionView MessageList
        {
            get
            {
                if (_messageList == null)
                    _messageList = CollectionViewSource.GetDefaultView(Messages);

                return _messageList;
            }
        }

        public ChatWindowDataContext()
        {
            _dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
            Client = new Client(IPAddress.Loopback, 8005);
            Receiver = null;
            MessageList.Filter = Filter;
        }

        private void OnRecevierChanged(UserListElement user)
        {
            if (user != null)
            {
                user.IsAnyNewMessage = false;
            }
            MessageList.Refresh();
        }

        private UserListElement FindUserByName(string userName)
        {
            return ConnectionList.SingleOrDefault((x) => x.UserName == userName);
        }

        private void ChangeUserIfFound(string userName, Action<UserListElement> action)
        {
            var user = FindUserByName(userName);
            if (user != null)
                action(user);
        }

        private bool Filter(object obj)
        {
            if (Receiver == null)
                return obj is TextMessage;

            if (!(obj is PersonalMessage pm))
                return false;

            return (pm.ReceiverName == Receiver?.UserName && pm.UserName == UserName) || (pm.UserName == Receiver?.UserName && pm.ReceiverName == UserName);
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

            var unc = owner.Resources["UserNameConverter"] as UserNameConverter;
            if (unc != null)
                unc.OwnName = UserName;

            ConnectingEventHandlers();

            foreach (var el in await Client.GetConnectionListAsync())
            {
                ConnectionList.Add(new UserListElement(el));
            }
        }

        private void ConnectingEventHandlers()
        {
            Client.ConnectionNotificationMessageReceived += (_, cnm) =>
            {
                var msg = (ConnectionNotificationMessage)cnm;
                _dispatcher.Invoke(() =>
                {
                    var el = FindUserByName(msg.UserName);
                    if (el != null)
                        el.IsConnected = true;
                    else ConnectionList.Add(new UserListElement(msg.UserName));
                });
            };

            Client.DisconnectionNotificationMessageReceived += (_, dnm) =>
            {
                var msg = (DisconnectionNotificationMessage)dnm;
                _dispatcher.Invoke(() => ChangeUserIfFound(msg.UserName, el => el.IsConnected = false));
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
                if (res.UserName != UserName)
                    _dispatcher.Invoke(() => Messages.Add(res));
            };

            Client.ServerStopped += (_, ss) =>
            {
                _dispatcher.Invoke(() =>
                {
                    Messages.Add(new TextMessage("The server has stopped", "Server", Guid.NewGuid()));
                    ConnectionList.Clear();
                });
            };

            Messages.CollectionChanged += (s, e) =>
            {
                var msg = Messages[Messages.Count - 1];
                if (msg is PersonalMessage pm && pm.UserName != Receiver?.UserName && pm.UserName != UserName)
                {
                    ChangeUserIfFound(pm.UserName, el =>
                    {
                        el.IsAnyNewMessage = true;
                        ConnectionList.Move(ConnectionList.IndexOf(el), 0);
                    });
                }
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
                Messages.Add(new PersonalMessage(UserName, Receiver.UserName, Message, Guid.NewGuid()));
                await Client.SendPersonallyMessage(Receiver.UserName, Message);
            }
            else
            {
                Messages.Add(new TextMessage(Message, UserName, Guid.NewGuid()));
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

            _dataContext.MessageList.CollectionChanged += (s, e) =>
            {
                MessageViewer.ScrollToBottom();
            };

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

