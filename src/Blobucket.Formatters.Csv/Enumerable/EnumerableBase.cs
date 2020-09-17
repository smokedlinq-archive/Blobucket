namespace Blobucket.Formatters.Enumerable
{
    #pragma warning disable S2743
    internal abstract class EnumerableBase<T>
        where T : class
    {
        protected static string TypeNotSupportedMessage => $"The type '{typeof(T).Name}' is not supported; supported types are: IEnumerable<>, IList<>, or an array.";
        protected static EnumerableType Type { get; private set; } = new EnumerableType(typeof(T));
        private static readonly EnumerableConverter __converter = new EnumerableConverter(Type);

        protected EnumerableBase()
        {
        }

        protected static T Convert(object obj)
            => (T)__converter.Convert(obj);
    }
}