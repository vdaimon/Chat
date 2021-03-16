using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class MessageBase : IGetBytes
    {
        public Communicator.MessageType MessageType { get; }
        public Guid TransactionId { get; }

        protected MessageBase(Stream stream, Communicator.MessageType msgType)
        {
            MessageType = msgType;
            TransactionId = stream.ReadGuid();
        }

        protected MessageBase(Communicator.MessageType msgType, Guid transactionId)
        {
            MessageType = msgType;
            TransactionId = transactionId;
        }
        public virtual void ToStream(Stream stream)
        {
            stream.WriteGuid(TransactionId);
        }
    }
}
