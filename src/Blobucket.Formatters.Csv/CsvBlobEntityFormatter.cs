using System;
using System.Collections.Generic;
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
        private readonly Encoding _encoding;
        private readonly Action<IReaderConfiguration>? _configureReader;
        private readonly Action<IWriterConfiguration>? _configureWriter;

        public CsvBlobEntityFormatter(Encoding? encoding = null, Action<IReaderConfiguration>? configureReader = null, Action<IWriterConfiguration>? configureWriter = null)
        {
            _encoding = encoding ?? DefaultEncoding;
            _configureReader = configureReader;
            _configureWriter = configureWriter;
        }

        public override async Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {   
            try
            {
                using (var reader = new StreamReader(stream, _encoding, true, 1024, true))
                using (var csv = new CsvReader(reader, true))
                {
                    _configureReader?.Invoke(csv.Configuration);

                    if (await csv.ReadAsync().ConfigureAwait(false))
                    {
                        csv.ReadHeader();

                        if (await csv.ReadAsync().ConfigureAwait(false))
                        {
                            return csv.GetRecord<T>();
                        }
                    }
                }

                # nullable disable
                return default;
                # nullable enable
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }

        public override async Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var stream = new MemoryStream();

                using (var writer = new StreamWriter(stream, _encoding, 1024, true))
                using (var csv = new CsvWriter(writer, true))
                {
                    _configureWriter?.Invoke(csv.Configuration);

                    csv.WriteHeader<T>();
                    await csv.NextRecordAsync().ConfigureAwait(false);
                    
                    csv.WriteRecord(entity);
                    await csv.NextRecordAsync().ConfigureAwait(false);
                }

                stream.Position = 0;

                return stream;
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }
    }
}
