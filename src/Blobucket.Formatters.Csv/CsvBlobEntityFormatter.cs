using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace Blobucket.Formatters
{
    public sealed class CsvBlobEntityFormatter : BlobEntityFormatter
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
        public static readonly CultureInfo DefaultCulture = CultureInfo.CurrentCulture;
        private const int DefaultBufferSize = 4096;

        private readonly Encoding _encoding;
        private readonly CultureInfo _culture;
        private readonly int _bufferSize;
        private readonly Action<IReaderConfiguration>? _configureReader;
        private readonly Action<IWriterConfiguration>? _configureWriter;

        public CsvBlobEntityFormatter(Encoding? encoding = null, CultureInfo? culture = null, bool hasHeader = false, int bufferSize = DefaultBufferSize, Action<IReaderConfiguration>? configureReader = null, Action<IWriterConfiguration>? configureWriter = null)
        {
            _encoding = encoding ?? DefaultEncoding;
            _culture = culture ?? DefaultCulture;
            _bufferSize = bufferSize;

            _configureReader = x => 
            {
                x.HasHeaderRecord = hasHeader;
                configureReader?.Invoke(x);
            };

            _configureWriter = x =>
            {
                x.HasHeaderRecord = hasHeader;
                configureWriter?.Invoke(x);
            };
        }

        public override Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            where T : class
        {   
            _ = stream ?? throw new ArgumentNullException(nameof(stream));
            return DeserializeAsyncInternal<T>(stream);
        }
        
        private static string NoRecordFoundMessage => "The blob does not contain a record.";

        private async Task<T> DeserializeAsyncInternal<T>(Stream stream)
            where T : class
        {
            try
            {
                using (var reader = new StreamReader(stream, _encoding, true, _bufferSize, true))
                using (var csv = new CsvReader(reader, _culture, true))
                {
                    _configureReader?.Invoke(csv.Configuration);

                    var serializer = CsvReaderFactory<T>.Create(csv);

                    if (csv.Configuration.HasHeaderRecord && await csv.ReadAsync().ConfigureAwait(false))
                    {
                        csv.ReadHeader();
                    }

                    var record = await serializer.GetRecordAsync().ConfigureAwait(false);

                    if (record is null)
                    {
                        throw new BlobEntityFormatterException(NoRecordFoundMessage);
                    }

                    return record;
                }
            }
            catch (BlobEntityFormatterException)
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
            _ = entity ?? throw new ArgumentNullException(nameof(entity));
            return SerializeAsyncInternal<T>(entity);
        }

        private async Task<Stream> SerializeAsyncInternal<T>(T entity)
            where T : class
        {
            try
            {
                var stream = new MemoryStream();

                using (var writer = new StreamWriter(stream, _encoding, _bufferSize, true))
                using (var csv = new CsvWriter(writer, _culture, true))
                {
                    _configureWriter?.Invoke(csv.Configuration);

                    var serializer = CsvWriterFactory<T>.Create(csv);

                    if (csv.Configuration.HasHeaderRecord)
                    {
                        serializer.WriteHeader(entity);
                        await csv.NextRecordAsync().ConfigureAwait(false);
                    }
                    
                    await serializer.WriteRecordAsync(entity).ConfigureAwait(false);
                }

                stream.Position = 0;

                return stream;
            }
            catch (BlobEntityFormatterException)
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
