using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public sealed class JsonBlobEntityFormatter : BlobEntityFormatter
    {
        private readonly JsonSerializerOptions? _options;

        public JsonBlobEntityFormatter(JsonSerializerOptions? options = default)
            => _options = options;

        public override Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            where T : class
        {
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            return DeserializeAsyncInternal<T>(stream, cancellationToken);
        }

        public async Task<T> DeserializeAsyncInternal<T>(Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, options: _options, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }

        public override Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            where T : class
        {
            _ = entity  ?? throw new ArgumentNullException(nameof(entity));
            return SerializeAsyncInternal<T>(entity, cancellationToken);
        }

        public async Task<Stream> SerializeAsyncInternal<T>(T entity, CancellationToken cancellationToken)
        {
            try
            {
                Stream stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, entity, options: _options, cancellationToken: cancellationToken).ConfigureAwait(false);
                stream.Position = 0;
                return stream;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }
    }
}