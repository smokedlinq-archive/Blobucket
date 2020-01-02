using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace Blobucket
{
    public static class BlobEntityContainerFactoryExtensions
    {
        public static Task<BlobEntityContainer<T>> CreateContainerForAsync<T>(this BlobEntityContainerFactory factory, Action<BlobEntityContainerOptionsBuilder<T>>? configure = null, PublicAccessType publicAccessType = PublicAccessType.None, CancellationToken cancellationToken = default)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return factory.CreateContainerForInternalAsync(configure, publicAccessType, cancellationToken);
        }

        private static async Task<BlobEntityContainer<T>> CreateContainerForInternalAsync<T>(this BlobEntityContainerFactory factory, Action<BlobEntityContainerOptionsBuilder<T>>? configure, PublicAccessType publicAccessType, CancellationToken cancellationToken)
        {
            var container = factory.GetContainerFor<T>(configure);
            await container.CreateIfNotExistsAsync(publicAccessType: publicAccessType, cancellationToken: cancellationToken).ConfigureAwait(false);
            return container;
        }
    }
}