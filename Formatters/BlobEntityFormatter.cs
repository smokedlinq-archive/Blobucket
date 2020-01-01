using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public abstract class BlobEntityFormatter
    {
        public abstract Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default);
        public abstract Task<Stream> SerializeAsync<T>(T entity, CancellationToken cancellationToken = default);
    }
}