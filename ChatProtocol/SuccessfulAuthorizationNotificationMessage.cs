using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class SuccessfulAuthorizationNotificationMessage : MessageBase
    {
        public bool IsSuccessful { get; }
        public string Message { get; }

        public SuccessfulAuthorizationNotificationMessage(Stream stream)
            : base(stream, Communicator.MessageType.SuccessfulAuthorizationNotification)
        {
                IsSuccessful = stream.ReadBool();
                Message = stream.ReadString();
        }
        public SuccessfulAuthorizationNotificationMessage(bool status, Guid transactionId, string message="")
            : base (Communicator.MessageType.SuccessfulAuthorizationNotification, transactionId)
        {
            IsSuccessful = status;
            Message = message;
        }
        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
            stream.WriteBool(IsSuccessful);
            stream.WriteString(Message);
        }
    }
}

