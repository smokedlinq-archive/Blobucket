using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityContainerFactoryOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public BlobEntityFormatter Formatter { get; set; } = BlobEntityFormatter.Default;
    }
}
