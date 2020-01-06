using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    internal interface ICsvReader<T>
        where T : class
    {
        Task<T?> GetRecordAsync();
    }
}