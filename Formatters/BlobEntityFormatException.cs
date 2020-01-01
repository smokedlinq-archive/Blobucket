using System;
using System.Runtime.Serialization;

namespace Blobucket.Formatters
{
    [Serializable]
    internal class BlobEntityFormatException : Exception
    {
        public BlobEntityFormatException()
        {
        }

        public BlobEntityFormatException(string message)
            : base(message)
        {
        }

        public BlobEntityFormatException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected BlobEntityFormatException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}