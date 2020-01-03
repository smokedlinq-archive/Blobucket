using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public abstract partial class BlobEntityFormatter
    {
        public static readonly BlobEntityFormatter Null = new NullBlobEntityFormatter();

        private class NullBlobEntityFormatter : BlobEntityFormatter
        {
            public override Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            {
                # nullable disable
                return Task.FromResult<T>(default);
                # nullable enable
            }

            public override Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(Stream.Null);
            }
        }
    }
}