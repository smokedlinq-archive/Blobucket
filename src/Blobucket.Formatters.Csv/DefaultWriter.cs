using System;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters
{
    internal class DefaultWriter<T> : ICsvWriter<T>
        where T : class
    {
        private readonly CsvWriter _writer;

        public DefaultWriter(CsvWriter writer)
            => _writer = writer ?? throw new ArgumentNullException(nameof(writer));

        public void WriteHeader(T entity)
            => _writer.WriteHeader<T>();

        public Task WriteRecordAsync(T entity)
        {
            _writer.WriteRecord(entity);
            return _writer.NextRecordAsync();
        }
    }
}