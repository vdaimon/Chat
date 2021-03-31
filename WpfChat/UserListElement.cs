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
        public string UserName { get; }
        public bool IsAnyNewMessage { get => _isAnyNewMessage; set => Set(ref _isAnyNewMessage, value); }

        public UserListElement(string userName)
        {
            UserName = userName;
        }
    }
}
