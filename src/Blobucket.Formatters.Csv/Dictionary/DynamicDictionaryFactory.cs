using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace Blobucket.Formatters.Dictionary
{
    internal class DynamicDictionaryFactory
    {
        private readonly Delegate _delegate;

        public DynamicDictionaryFactory(DictionaryType type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            if (type.Dictionary is null || type.Key is null || type.Value is null || !type.Dictionary.IsAssignableFrom(type.Type))
            {
                #pragma warning disable S3626
                _delegate = new Func<object, object>(_ => throw new NotSupportedException());
                #pragma warning restore S3626
            }
            else
            {
                var dictionary = Expression.Parameter(type.Dictionary, "dictionary");
                var dynamicDictionary = typeof(DynamicDictionaryObject);
                var from = Expression.Call(dynamicDictionary, "From", new Type[] { type.Key, type.Value }, dictionary);
                var func = typeof(Func<,>).MakeGenericType(type.Dictionary, dynamicDictionary);
                
                _delegate = Expression.Lambda(func, from, dictionary).Compile();
            }
        }

        public dynamic ToDynamicObject(object obj)
            => _delegate.DynamicInvoke(obj);

        class DynamicDictionaryObject : DynamicObject
        {
            private readonly Dictionary<string, object?> _dictionary = new Dictionary<string, object?>();

            public static DynamicDictionaryObject From<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
            {
                var obj = new DynamicDictionaryObject();
                foreach(var kvp in dictionary)
                {
                    if (!(kvp.Key is null))
                    {
                        obj._dictionary.Add(kvp.Key.ToString(), kvp.Value);
                    }
                }
                return obj;
            }

            public override IEnumerable<string> GetDynamicMemberNames()
                => _dictionary.Keys;

            public override bool TryGetMember(GetMemberBinder binder, out object? result)
                => _dictionary.TryGetValue(binder.Name, out result);
        }
    }
}