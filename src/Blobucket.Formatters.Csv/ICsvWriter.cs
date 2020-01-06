using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    internal interface ICsvWriter<in T>
    {
        void WriteHeader(T entity);
        Task WriteRecordAsync(T entity);
    }
}