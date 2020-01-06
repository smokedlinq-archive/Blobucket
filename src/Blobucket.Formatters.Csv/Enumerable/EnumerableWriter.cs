using System;
using System.Collections;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters.Enumerable
{
    internal class EnumerableWriter<T> : EnumerableBase<T>, ICsvWriter<T>
        where T : class
    {
        private readonly CsvWriter _writer;

        public EnumerableWriter(CsvWriter writer)
            => _writer = writer ?? throw new ArgumentNullException(nameof(writer));

        public static bool IsSupported { get; private set; }
            = (Type.Enumerable?.IsAssignableFrom(typeof(T)) ?? false)
            || Type.IsArray
            || (Type.List?.IsAssignableFrom(typeof(T)) ?? false);

        public void WriteHeader(T entity)
        {
            if (!IsSupported)
            {
                throw new BlobEntityFormatterException(TypeNotSupportedMessage);
            }

            _writer.WriteHeader(Type.Element);
        }

        public Task WriteRecordAsync(T entity)
        {
            if (!IsSupported)
            {
                throw new BlobEntityFormatterException(TypeNotSupportedMessage);
            }

            _writer.WriteRecords((IEnumerable)entity);
            return Task.CompletedTask;
        }
    }
}