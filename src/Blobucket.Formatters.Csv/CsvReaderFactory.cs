using System;
using System.Collections;
using Blobucket.Formatters.Dictionary;
using Blobucket.Formatters.Dynamic;
using Blobucket.Formatters.Enumerable;
using CsvHelper;

namespace Blobucket.Formatters
{
    internal static class CsvReaderFactory<T>
        where T : class
    {
        public static ICsvReader<T> Create(CsvReader reader)
            => __delegate(reader);

        private static readonly Func<CsvReader, ICsvReader<T>> __delegate = GetReaderDelegate();

        private static Func<CsvReader, ICsvReader<T>> GetReaderDelegate()
        {
            if (DynamicReader<T>.IsSupported)
            {
                return x => new DynamicReader<T>(x);
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (EnumerableReader<T>.IsSupported)
                {
                    return x => new EnumerableReader<T>(x);
                }

                if (DictionaryReader<T>.IsSupported)
                {
                    return x => new DictionaryReader<T>(x);
                }
            }

            return x => new DefaultReader<T>(x);
        }
    }
}