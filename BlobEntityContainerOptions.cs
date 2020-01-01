using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityContainerOptions<T>
    {
        public string ContainerName { get; set; } = typeof(T).Name;
        public BlobEntityFormatter Formatter { get; set; } = new JsonBlobEntityFormatter();
    }
}
