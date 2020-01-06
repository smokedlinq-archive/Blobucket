using System.Collections;
using Blobucket.Formatters.Dictionary;
using Blobucket.Formatters.Dynamic;
using Blobucket.Formatters.Enumerable;
using CsvHelper;

namespace Blobucket.Formatters
{
    internal static class CsvSerializerFactory
    {
        public static ICsvReader<T> CreateFor<T>(CsvReader reader)
            where T : class
        {
            if (DynamicReader<T>.IsSupported)
            {
                return new DynamicReader<T>(reader);
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (EnumerableReader<T>.IsSupported)
                {
                    return new EnumerableReader<T>(reader);
                }

                if (DictionaryReader<T>.IsSupported)
                {
                    return new DictionaryReader<T>(reader);
                }
            }

            return new DefaultReader<T>(reader);
        }

        public static ICsvWriter<T> CreateFor<T>(CsvWriter writer)
            where T : class
        {
            if (DynamicWriter<T>.IsSupported)
            {
                return new DynamicWriter<T>(writer);
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (EnumerableWriter<T>.IsSupported)
                {
                    return new EnumerableWriter<T>(writer);
                }

                if (DictionaryWriter<T>.IsSupported)
                {
                    return new DictionaryWriter<T>(writer);
                }
            }

            return new DefaultWriter<T>(writer);
        }
    }
}