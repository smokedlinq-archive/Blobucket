using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public abstract partial class BlobEntityFormatter
    {        
        public abstract Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default);
        public abstract Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default);
    }
}