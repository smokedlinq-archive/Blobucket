using Blobucket.Formatters;

namespace Blobucket
{
    public sealed class BlobEntityOptions
    {
        public BlobEntityFormatter Formatter { get; set; } = BlobEntityFormatter.Null;
    }
}