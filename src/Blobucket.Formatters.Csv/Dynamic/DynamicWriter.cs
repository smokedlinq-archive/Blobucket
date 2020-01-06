using System;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters.Dynamic
{
    public class DynamicWriter<T> : DynamicBase<T>, ICsvWriter<T>
        where T : class
    {
        private readonly CsvWriter _writer;

        public DynamicWriter(CsvWriter writer)
            => _writer = writer ?? throw new ArgumentNullException(nameof(writer));

        public void WriteHeader(T entity)
        {
            _writer.WriteDynamicHeader((dynamic)entity);
        }

        public Task WriteRecordAsync(T entity)
        {
            _writer.WriteRecord<dynamic>(entity);
            return _writer.NextRecordAsync();
        }
    }
}