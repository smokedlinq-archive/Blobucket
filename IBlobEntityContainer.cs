using System.Threading;
using System.Threading.Tasks;
using Blobucket.Formatters;

namespace Blobucket
{
    public interface IBlobEntityContainer<T>
    {
        Task<T> GetAsync(string id, BlobEntityOptions? options = null, CancellationToken cancellationToken = default);
        Task SetAsync(string id, T entity, BlobEntityOptions? options = null, CancellationToken cancellationToken = default);
        Task DeleteAsync(string id, BlobEntityOptions? options = null, CancellationToken cancellationToken = default);
    }
}
