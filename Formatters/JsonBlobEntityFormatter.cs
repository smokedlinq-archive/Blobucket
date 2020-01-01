using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public class JsonBlobEntityFormatter : BlobEntityFormatter
    {
        private readonly JsonSerializerOptions? _options;

        public JsonBlobEntityFormatter(JsonSerializerOptions? options = default)
            => _options = options;

        public override async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<T>(stream, options: _options, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatException(ex.Message, ex);
            }
        }

        public override async Task<Stream> SerializeAsync<T>(T entity, CancellationToken cancellationToken = default)
        {
            try
            {
                Stream stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, entity, options: _options, cancellationToken: cancellationToken).ConfigureAwait(false);
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatException(ex.Message, ex);
            }
        }
    }
}