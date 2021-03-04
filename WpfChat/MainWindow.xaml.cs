using ChatProtocol;
using System.Windows;
using System.Windows.Controls;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Show();
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var cw = new ChatWindow(this.Username.Text);
            cw.Show();
            
            Close();
        }
    }
}
