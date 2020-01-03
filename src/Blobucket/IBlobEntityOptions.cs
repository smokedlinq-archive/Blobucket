using Blobucket.Formatters;

namespace Blobucket
{
    public interface IBlobEntityOptions
    {
        BlobEntityFormatter Formatter { get; set; }
    }
}