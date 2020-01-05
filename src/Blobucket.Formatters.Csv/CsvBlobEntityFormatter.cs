using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        private readonly bool _hasHeader;
        private readonly Action<IReaderConfiguration>? _configureReader;
        private readonly Action<IWriterConfiguration>? _configureWriter;

        public CsvBlobEntityFormatter(Encoding? encoding = null, bool hasHeader = false, Action<IReaderConfiguration>? configureReader = null, Action<IWriterConfiguration>? configureWriter = null)
        {
            _encoding = encoding ?? DefaultEncoding;
            _hasHeader = hasHeader;
            _configureReader = configureReader;
            _configureWriter = configureWriter;
        }

        public override Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {   
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return DeserializeAsyncInternal<T>(stream);
        }

        
        private static string NoRecordFoundMessage => "The blob does not contain a record.";
        private static string EnumerableTypeNotSupportedMessage => "Could not convert the records to type '{0}'; supported types are: IEnumerable<>, IList<>, or an array.";

        public async Task<T> DeserializeAsyncInternal<T>(Stream stream)
        {
            try
            {
                using (var reader = new StreamReader(stream, _encoding, true, 1024, true))
                using (var csv = new CsvReader(reader, true))
                {
                    _configureReader?.Invoke(csv.Configuration);

                    if (_hasHeader && await csv.ReadAsync().ConfigureAwait(false))
                    {
                        csv.ReadHeader();
                    }
                    else if (!_hasHeader)
                    {
                        csv.Configuration.HasHeaderRecord = false;
                    }

                    if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                    {
                        var elementType = typeof(T).GetElementType() ?? typeof(T).GetGenericArguments().FirstOrDefault();

                        if (elementType != null)
                        {
                            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
                            var records = GetRecords(csv, elementType, enumerableType);
                                                        
                            if (enumerableType == typeof(T) || typeof(T).IsArray)
                            {
                                return AsEnumerableType<T>(enumerableType, elementType, "ToArray", records);
                            }

                            var listType = typeof(IList<>).MakeGenericType(elementType);

                            if (listType == typeof(T))
                            {
                                return AsEnumerableType<T>(enumerableType, elementType, "ToList", records);
                            }
                        }

                        throw new BlobEntityFormatterException(string.Format(CultureInfo.CurrentCulture, EnumerableTypeNotSupportedMessage, typeof(T).Name));
                    }
                    else
                    {
                        if (await csv.ReadAsync().ConfigureAwait(false))
                        {
                            return csv.GetRecord<T>();
                        }
                    }
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

            throw new BlobEntityFormatterException(NoRecordFoundMessage);
        }

        private static object GetRecords(CsvReader reader, Type type, Type enumerable)
        {
            var call = Expression.Call(Expression.Constant(reader), reader.GetType().GetMethod("GetRecords", 1, Array.Empty<Type>()).MakeGenericMethod(type));
            var getRecords = typeof(Func<>).MakeGenericType(enumerable);
            return Expression.Lambda(getRecords, call).Compile().DynamicInvoke();
        }

        private static T AsEnumerableType<T>(Type enumerable, Type element, string methodName, object records)
        {
            var source = Expression.Parameter(enumerable, "source");
            var call = Expression.Call(typeof(Enumerable), methodName, new[] { element }, source);
            var toArray = typeof(Func<,>).MakeGenericType(enumerable, typeof(T));
            return (T)Expression.Lambda(toArray, call, source).Compile().DynamicInvoke(records);
        }

        public override Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return SerializeAsyncInternal<T>(entity);
        }

        private async Task<Stream> SerializeAsyncInternal<T>(T entity)
            where T : class
        {
            try
            {
                var stream = new MemoryStream();

                using (var writer = new StreamWriter(stream, _encoding, 1024, true))
                using (var csv = new CsvWriter(writer, true))
                {
                    _configureWriter?.Invoke(csv.Configuration);

                    if (_hasHeader)
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                        {
                            var elementType = typeof(T).GetElementType() ?? typeof(T).GetGenericArguments().FirstOrDefault();

                            if (elementType == null)
                            {
                                throw new BlobEntityFormatterException(string.Format(CultureInfo.CurrentCulture, EnumerableTypeNotSupportedMessage, typeof(T).Name));
                            }

                            csv.WriteHeader(elementType);
                        }
                        else
                        {
                            csv.WriteHeader<T>();
                        }

                        await csv.NextRecordAsync().ConfigureAwait(false);
                    }
                    
                    if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
                    {
                        csv.WriteRecords((IEnumerable)entity);
                    }
                    else
                    {
                        csv.WriteRecord(entity);
                    }

                    await csv.NextRecordAsync().ConfigureAwait(false);
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
