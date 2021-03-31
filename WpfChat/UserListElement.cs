using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    public class UserListElement : INPC
    {
        private bool _isAnyNewMessage;
        private bool _isConnected;
        public string UserName { get; }
        public bool IsConnected { get => _isConnected; set => Set(ref _isConnected, value); }
        public bool IsAnyNewMessage { get => _isAnyNewMessage; set => Set(ref _isAnyNewMessage, value); }

        public UserListElement(string userName)
        {
            UserName = userName;
            IsConnected = true;
        }
    }
}
