using System;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters.Dynamic
{
    public class DynamicReader<T> : DynamicBase<T>, ICsvReader<T>
        where T : class
    {
        private readonly CsvReader _reader;

        public DynamicReader(CsvReader reader)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        public async Task<T?> GetRecordAsync()
        {
            if (await _reader.ReadAsync().ConfigureAwait(false))
            {
                return _reader.GetRecord<dynamic>();
            }
            
            return default;
        }
    }
}