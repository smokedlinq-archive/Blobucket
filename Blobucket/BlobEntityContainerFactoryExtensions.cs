using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket
{
    public static class BlobEntityContainerFactoryExtensions
    {
        public static async Task<BlobEntityContainer<T>> CreateContainerForAsync<T>(this BlobEntityContainerFactory factory, Action<BlobEntityContainerOptionsBuilder<T>>? configure = null, CancellationToken cancellationToken = default)
        {
            var container = factory.GetContainerFor<T>(configure);
            await container.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);
            return container;
        }
    }
}