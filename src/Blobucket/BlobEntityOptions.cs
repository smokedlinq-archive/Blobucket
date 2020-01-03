using Blobucket.Formatters;

namespace Blobucket
{
    public sealed class BlobEntityOptions : IBlobEntityOptions
    {
        public BlobEntityFormatter Formatter { get; set; } = BlobEntityFormatter.Null;
    }
}