using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace Blobucket
{
    public interface IBlobEntity<T>
    {
        Task<T> GetAsync(CancellationToken cancellationToken = default);
        Task SetAsync(T entity, bool overwrite = true, CancellationToken cancellationToken = default);
        Task DeleteAsync(DeleteSnapshotsOption snapshotsOption = DeleteSnapshotsOption.None, CancellationToken cancellationToken = default);
        Task CreateSnapshotAsync(IDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
    }
}