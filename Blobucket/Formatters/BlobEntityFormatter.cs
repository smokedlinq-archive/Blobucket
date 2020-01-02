using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public abstract class BlobEntityFormatter
    {
        private static readonly BlobEntityFormatter __default = new JsonBlobEntityFormatter();
        public static BlobEntityFormatter Default => __default;
        
        public abstract Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default);
        public abstract Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default);
    }
}