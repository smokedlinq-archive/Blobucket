using System;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters.Dictionary
{
    internal class DictionaryReader<T> : DictionaryBase<T>, ICsvReader<T>
        where T : class
    {
        private readonly CsvReader _reader;

        public DictionaryReader(CsvReader reader)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        public static bool IsSupported { get; private set; } = typeof(T) == Type.DynamicDictionary;
        
        public async Task<T?> GetRecordAsync()
        {
            if (!IsSupported)
            {
                throw new NotSupportedException(TypeNotSupportedMessage);
            }

            if (await _reader.ReadAsync().ConfigureAwait(false))
            {
                return _reader.GetRecord<dynamic>() as T;
            }

            return default;
        }
    }
}