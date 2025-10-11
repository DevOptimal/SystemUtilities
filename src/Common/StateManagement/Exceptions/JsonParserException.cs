using System;
using System.Runtime.Serialization;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Exceptions
{
    public class JsonParserException : Exception
    {
        public JsonParserException()
        {
        }

        public JsonParserException(string message) : base(message)
        {
        }

        public JsonParserException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JsonParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
