using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFClient;

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
