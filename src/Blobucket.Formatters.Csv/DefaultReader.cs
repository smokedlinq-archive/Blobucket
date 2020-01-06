using System;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters
{
    internal class DefaultReader<T> : ICsvReader<T>
        where T : class
    {
        private readonly CsvReader _reader;

        public DefaultReader(CsvReader reader)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        
        public async Task<T?> GetRecordAsync()
        {
            if (await _reader.ReadAsync().ConfigureAwait(false))
            {
                return _reader.GetRecord<T>();
            }
            
            return default;
        }
    }
}