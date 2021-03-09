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

        public SuccessfulAuthorizationNotificationMessage(byte[] statusAndMessage)
        {
            using (MemoryStream stream = new MemoryStream(statusAndMessage))
            {
                IsSuccessful = stream.ReadBool();
                Message = stream.ReadString();
            }
        }
        public SuccessfulAuthorizationNotificationMessage(bool status, string message="")
        {
            IsSuccessful = status;
            Message = message;
        }
        public byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.WriteBool(IsSuccessful);
                stream.WriteString(Message);
                return stream.ToArray();
            }
        }
    }
}

