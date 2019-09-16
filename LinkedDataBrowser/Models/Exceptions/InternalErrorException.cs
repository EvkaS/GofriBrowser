using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LinkedDataBrowser.Models.Exceptions
{
    public class InternalErrorException : Exception
    {
        public InternalErrorException()
        {
        }

        public InternalErrorException(string message) : base(message)
        {
        }

        public InternalErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InternalErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
