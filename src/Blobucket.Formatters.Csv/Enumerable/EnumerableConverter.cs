using System;
using System.Linq.Expressions;

namespace Blobucket.Formatters.Enumerable
{
    internal class EnumerableConverter
    {
        private readonly Delegate _delegate;

        public EnumerableConverter(EnumerableType type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.Element is null || type.Enumerable is null || type.List is null)
            {
                _delegate = new Func<object, object>(_ => throw new NotSupportedException());
            }
            else
            {
                var methodName = (type.Type == type.Array) ? "ToArray" : "ToList";
                var source = Expression.Parameter(type.Enumerable, "source");
                var call = Expression.Call(typeof(System.Linq.Enumerable), methodName, new[] { type.Element }, source);
                var lambda = typeof(Func<,>).MakeGenericType(type.Enumerable, type.Type);

                _delegate = Expression.Lambda(lambda, call, source).Compile();
            }
        }

        public object Convert(object obj)
            => _delegate.DynamicInvoke(obj);
    }
}