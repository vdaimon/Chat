using ChatProtocol;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace WPFClient
{
    public class AuthorizationWindowDataContext: INPC
    {
        private bool _enableConnectButtonFlag;
        private string _userName;

        public string UserName { get => _userName; set { Set(ref _userName, value, ()=> EnableConnectButtonFlag = !string.IsNullOrEmpty(value)); } }
        public string Text { get; set; }
        public IPAddress IP { get; set; }
        public int Port { get; set; }
        public bool EnableConnectButtonFlag { get => _enableConnectButtonFlag; set => Set(ref _enableConnectButtonFlag, value); }

        public AuthorizationWindowDataContext()
        {
            IP = IPAddress.Loopback;
            Port = 8005;
            EnableConnectButtonFlag = false;
        }
        public void OpenSettings(Window owner)
        {
            var st = new SettingsWindow();
            st.Owner = owner;
            st.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            st.ShowDialog();
            IP = st.dataContext.IP;
            Port = st.dataContext.Port;
        }
    }
    /// <summary>
    /// Interaction logic for AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        
        public AuthorizationWindowDataContext dataContext = new AuthorizationWindowDataContext();
        public AuthorizationWindow()
        {
            InitializeComponent();
            DataContext = dataContext;
            UserNameBox.Focus();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            dataContext.OpenSettings(this);
        }
    }
}
