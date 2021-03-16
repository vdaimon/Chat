using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ServerResponse : MessageBase
    {
        public enum ResponseType : byte
        {
            AuthorizationSuccessfully,
            NameAlreadyInUse,
            MessageSendSuccessfully,
            RecipientNotFound,
        }
        public ResponseType Response {get;}
        public ServerResponse(ResponseType response, Guid transactionId)
            : base(Communicator.MessageType.ServerResponse, transactionId)
        {
            Response = response;
        }

        public ServerResponse(Stream stream)
            : base(stream, Communicator.MessageType.ServerResponse)
        {
            Response = (ResponseType)stream.ReadByte();
        }

        public override void ToStream(Stream stream)
        {
            base.ToStream(stream);
            stream.WriteByte((byte)Response);
        }
    }
}
