using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    class SuccessfulAuthorizationNotificationMessage : IGetBytes
    {
        public bool IsSuccessful { get; }
        public Communicator.MessageType MessageType => Communicator.MessageType.SuccessfulAuthorizationNotification;

        public SuccessfulAuthorizationNotificationMessage(bool status)
        {
            IsSuccessful = status;
        }
        public byte[] GetBytes()
        {
            return new byte[] { };
        }
    }
}
