using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public sealed class MetadataBlobEntityFormatter : BlobEntityFormatter
    {
        private readonly BlobEntityFormatter _formatter;
        private readonly Action<object, IDictionary<string, string>> _callback;

        public MetadataBlobEntityFormatter(BlobEntityFormatter formatter, IDictionary<string, string> metadata)
        {
            _ = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _callback = (_, m) => 
            {
                foreach(var item in metadata)
                {
                    m[item.Key] = item.Value;
                }
            };
        }

        public MetadataBlobEntityFormatter(BlobEntityFormatter formatter, Action<IDictionary<string, string>> callback)
        {
            _ = callback ?? throw new ArgumentNullException(nameof(callback));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _callback = (_, metadata) => callback(metadata);
        }

        public MetadataBlobEntityFormatter(BlobEntityFormatter formatter, Action<object, IDictionary<string, string>> callback)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public override Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            => _formatter.DeserializeAsync<T>(stream, metadata, cancellationToken);

        public override Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            _callback(entity, metadata);
            return _formatter.SerializeAsync(entity, metadata, cancellationToken);
        }
    }
}