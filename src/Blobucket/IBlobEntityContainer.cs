using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket.Builders;

namespace Blobucket
{
    public interface IBlobEntityContainer<T>
        where T : class
    {
        Task CreateIfNotExistsAsync(PublicAccessType publicAccessType = PublicAccessType.None, CancellationToken cancellationToken = default);
        IBlobEntity<T> GetBlobEntity(string id, Action<IBlobEntityOptionsBuilder>? configure = null);
    }
}