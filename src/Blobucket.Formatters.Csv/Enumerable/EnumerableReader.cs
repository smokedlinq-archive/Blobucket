using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CsvHelper;

namespace Blobucket.Formatters.Enumerable
{
    internal class EnumerableReader<T> : EnumerableBase<T>, ICsvReader<T>
        where T : class
    {
        private readonly CsvReader _reader;

        public EnumerableReader(CsvReader reader)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        public static bool IsSupported { get; private set; }
            = typeof(T) == Type.Enumerable
            || Type.IsArray
            || typeof(T) == Type.List;

        public Task<T?> GetRecordAsync()
        {            
            if (!IsSupported)
            {
                throw new NotSupportedException(TypeNotSupportedMessage);
            }

            var method = _reader.GetType().GetMethod("GetRecords", 1, Array.Empty<Type>()).MakeGenericMethod(Type.Element);
            var call = Expression.Call(Expression.Constant(_reader), method);
            var lambda = typeof(Func<>).MakeGenericType(Type.Enumerable);
            var records = Expression.Lambda(lambda, call).Compile().DynamicInvoke();

            return Task.FromResult<T?>(Convert(records));        
        }
    }
}