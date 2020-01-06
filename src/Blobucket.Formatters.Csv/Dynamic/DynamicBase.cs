using System.Dynamic;

namespace Blobucket.Formatters.Dynamic
{
    #pragma warning disable CA1000
    public abstract class DynamicBase<T>
        where T : class
    {
        public static bool IsSupported { get; private set; } = typeof(IDynamicMetaObjectProvider).IsAssignableFrom(typeof(T));

        protected DynamicBase()
        {
        }
    }
}