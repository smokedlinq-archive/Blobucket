using System;
using System.Collections.Generic;

namespace Blobucket.Formatters.Dictionary
{
    internal class DictionaryType
    {
        public Type Type { get; private set; }
        public Type? Key { get; private set; }
        public Type? Value { get; private set; }
        public Type? Dictionary { get; private set; }
        public Type? DynamicDictionary { get; private set; } = typeof(IDictionary<string, object>);

        public DictionaryType(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));

            var genericArguments = type.GetGenericArguments();

            if (genericArguments.Length == 2)
            {
                Key = genericArguments[0];
                Value = genericArguments[1];
                Dictionary = typeof(IDictionary<,>).MakeGenericType(Key, Value);
            }
        }
    }
}