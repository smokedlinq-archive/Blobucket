using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityOptions
    {
        public BlobEntityFormatter Formatter { get; set; } = new JsonBlobEntityFormatter();
    }
}