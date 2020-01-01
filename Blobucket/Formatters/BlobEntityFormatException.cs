using System;
using System.Runtime.Serialization;

namespace Blobucket.Formatters
{
    [Serializable]
    public class BlobEntityFormatterException : Exception
    {
        public BlobEntityFormatterException()
        {
        }

        public BlobEntityFormatterException(string message)
            : base(message)
        {
        }

        public BlobEntityFormatterException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected BlobEntityFormatterException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}