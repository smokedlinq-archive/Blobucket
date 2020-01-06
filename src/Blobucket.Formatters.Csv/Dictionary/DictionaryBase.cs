namespace Blobucket.Formatters.Dictionary
{
    #pragma warning disable S2743
    internal abstract class DictionaryBase<T>
    {
        protected static string TypeNotSupportedMessage => $"The type '{typeof(T).Name}' is not supported, only IDictionary<string, object> types can be dictionaries used with the CSV formatter.";
        protected static DictionaryType Type { get; private set; } = new DictionaryType(typeof(T));
        private static readonly DynamicDictionaryFactory __factory = new DynamicDictionaryFactory(Type);
        
        protected dynamic ToDynamicObject(object obj)
            => __factory.ToDynamicObject(obj);
    }
}