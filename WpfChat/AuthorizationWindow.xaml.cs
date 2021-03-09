using ChatProtocol;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPFClient
{
    public class AuthorizationWindowDataContext
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public AuthorizationWindowDataContext()
        {

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
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
