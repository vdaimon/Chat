using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ServerExceptionMessage : MessageBase
    {
        public enum ServerError: byte
        {
            FailedToSendMessage,
            FailedToAutorize,
        }
        public string Message { get; }
        public ServerError ExceptionType { get; }

        public ServerExceptionMessage(ServerError type, string message, Guid transactionId)
            : base(Communicator.MessageType.ServerException, transactionId)
        {
            ExceptionType = type;
            Message = message;
        }

        public ServerExceptionMessage(Stream stream)
            : base (stream, Communicator.MessageType.ServerException)
        {
            ExceptionType = (ServerError)stream.ReadByte();
            Message = stream.ReadString();
        }
        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
            stream.WriteByte((byte)ExceptionType);
            stream.WriteString(Message);
        }
    }
}
