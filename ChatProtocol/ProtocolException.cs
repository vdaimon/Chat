using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatProtocol
{
    public class ProtocolException : Exception
    {
        public Error ErrorType { get; }
        public enum Error
        {
            FailedToRead,
            NegativeMessageLength,
            TooBigMessageLength,
            MessageLengthCRC,
            MessageCRC,
        }

        private static Dictionary<Error, string> _errorMessages = new Dictionary<Error, string> 
        {
            {Error.FailedToRead, "Failed to read"},
            {Error.NegativeMessageLength, "Negative message length"},
            {Error.TooBigMessageLength, "Too big message length"},
            {Error.MessageLengthCRC, "Invalid message length CRC"},
            {Error.MessageCRC, "Invalid message CRC"},
        };
       

        public ProtocolException(Error error)
           : base (_errorMessages[error])
        {
            ErrorType = error;
        }
    }
}
