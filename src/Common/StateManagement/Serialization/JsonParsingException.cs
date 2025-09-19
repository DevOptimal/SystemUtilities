using System;
using System.Runtime.Serialization;

namespace DevOptimal.SystemUtilities.Common.StateManagement.Serialization
{
    public class JsonParsingException : Exception
    {
        public JsonParsingException()
        {
        }

        public JsonParsingException(string message) : base(message)
        {
        }

        public JsonParsingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JsonParsingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
