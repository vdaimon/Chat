using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class SuccessfulAuthorizationNotificationMessage : IGetBytes
    {
        public bool IsSuccessful { get; }
        public string Message { get; }
        public Communicator.MessageType MessageType => Communicator.MessageType.SuccessfulAuthorizationNotification;

        public SuccessfulAuthorizationNotificationMessage(MemoryStream packet)
        {
                IsSuccessful = packet.ReadBool();
                Message = packet.ReadString();
        }
        public SuccessfulAuthorizationNotificationMessage(bool status, string message="")
        {
            IsSuccessful = status;
            Message = message;
        }
        public void GetBytes(MemoryStream stream)
        {
                stream.WriteBool(IsSuccessful);
                stream.WriteString(Message);
        }
    }
}

