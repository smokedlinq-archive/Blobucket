using Blobucket.Formatters;

namespace Blobucket.Builders
{
    public interface IBlobEntityContainerOptionsBuilder
    {
        IBlobEntityContainerOptionsBuilder UseContainerName<T>();
        IBlobEntityContainerOptionsBuilder UseContainerName(string containerName);
        IBlobEntityContainerOptionsBuilder UseFormatter<T>(T formatter)
            where T : BlobEntityFormatter;
    }
}