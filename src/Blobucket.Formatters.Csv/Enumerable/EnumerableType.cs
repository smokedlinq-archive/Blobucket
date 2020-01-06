using System;
using System.Collections.Generic;

namespace Blobucket.Formatters.Enumerable
{
    internal class EnumerableType
    {
        public Type Type { get; private set; }
        public Type? Element { get; private set; }
        public Type? Array { get; private set; }
        public Type? Enumerable { get; private set; }
        public Type? List { get; private set; }
        public bool IsArray => Type.IsArray;

        public EnumerableType(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));

            if (type.IsArray)
            {
                Element = type.GetElementType();
            }
            else
            {
                var genericArguments = type.GetGenericArguments();

                if (genericArguments.Length == 1)
                {
                    Element = genericArguments[0];
                }
            }

            Array = Element?.MakeArrayType();
            Enumerable = Element is null ? null : typeof(IEnumerable<>).MakeGenericType(Element);
            List = Element is null ? null : typeof(IList<>).MakeGenericType(Element);
        }
    }
}