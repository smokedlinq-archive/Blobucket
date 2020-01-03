using Blobucket.Formatters;

namespace Blobucket.Builders
{
    public interface IBlobEntityOptionsBuilder
    {
        IBlobEntityOptionsBuilder UseFormatter<T>(T formatter) 
            where T : BlobEntityFormatter;
    }
}