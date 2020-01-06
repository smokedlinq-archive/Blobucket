using System;
using System.Collections;
using Blobucket.Formatters.Dictionary;
using Blobucket.Formatters.Dynamic;
using Blobucket.Formatters.Enumerable;
using CsvHelper;

namespace Blobucket.Formatters
{
    internal static class CsvWriterFactory<T>
        where T : class
    {
        public static ICsvWriter<T> Create(CsvWriter reader)
            => __delegate(reader);

        private static readonly Func<CsvWriter, ICsvWriter<T>> __delegate = GetWriterDelegate();

        private static Func<CsvWriter, ICsvWriter<T>> GetWriterDelegate()
        {
            if (DynamicWriter<T>.IsSupported)
            {
                return x => new DynamicWriter<T>(x);
            }

            if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
            {
                if (EnumerableWriter<T>.IsSupported)
                {
                    return x => new EnumerableWriter<T>(x);
                }

                if (DictionaryWriter<T>.IsSupported)
                {
                    return x => new DictionaryWriter<T>(x);
                }
            }

            return x => new DefaultWriter<T>(x);
        }
    }
}