using System.Net;
using System.Windows;

namespace WPFClient
{
    public class SettingsWindowDataContext: INPC
    {
        private IPAddress _ip;
        private int _port;
        public IPAddress IP { get => _ip; set => Set(ref _ip, value); }
        public int Port { get => _port; set => Set(ref _port, value); }

        public SettingsWindowDataContext()
        {
            IP = IPAddress.Loopback;
            Port = 8005;
        }
    }

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindowDataContext dataContext = new SettingsWindowDataContext();
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
