using System;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters.Dictionary
{
    internal class DictionaryWriter<T> : DictionaryBase<T>, ICsvWriter<T>
        where T : class
    {
        private readonly CsvWriter _writer;

        public DictionaryWriter(CsvWriter writer)
            => _writer = writer ?? throw new ArgumentNullException(nameof(writer));

        public static bool IsSupported { get; private set; } = Type.Dictionary?.IsAssignableFrom(typeof(T)) ?? false;

        public void WriteHeader(T entity)
        {
            if (!IsSupported)
            {
                throw new NotSupportedException(TypeNotSupportedMessage);
            }

            var expando = ToDynamicObject(entity);
            _writer.WriteDynamicHeader(expando);
        }

        public Task WriteRecordAsync(T entity)
        {
            if (!IsSupported)
            {
                throw new NotSupportedException(TypeNotSupportedMessage);
            }

            var expando = ToDynamicObject(entity);
            _writer.WriteRecord<dynamic>(expando);
            return _writer.NextRecordAsync();
        }
    }
}