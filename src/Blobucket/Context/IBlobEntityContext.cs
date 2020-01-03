using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace Blobucket.Context
{
    public interface IBlobEntityContext
    {
        Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string id, T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync<T>(string id, DeleteSnapshotsOption snapshotOption = DeleteSnapshotsOption.None, CancellationToken cancellationToken = default);
        Task CreateSnapshotAsync<T>(string id, IDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    }
}
