using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace LinkedDataBrowser.Models.Exceptions
{
    public class BrowserException : Exception
    {
        public BrowserException()
        {
        }

        public BrowserException(string message) : base(message)
        {
        }

        public BrowserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BrowserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
