using System;
using System.Runtime.Serialization;

namespace Blobucket
{
    [Serializable]
    internal class BlobEntityNotFoundException : Exception
    {
        public BlobEntityNotFoundException()
        {
        }

        public BlobEntityNotFoundException(string message) 
            : base(message)
        {
        }

        public BlobEntityNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected BlobEntityNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}