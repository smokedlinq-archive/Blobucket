using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket.Builders;

namespace Blobucket
{
    public static class BlobEntityContainerFactoryExtensions
    {
        public static Task<IBlobEntityContainer<T>> CreateContainerForAsync<T>(this IBlobEntityContainerFactory factory, Action<IBlobEntityContainerOptionsBuilder>? configure = null, PublicAccessType publicAccessType = PublicAccessType.None, CancellationToken cancellationToken = default)
            where T : class
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            return factory.CreateContainerForInternalAsync<T>(configure, publicAccessType, cancellationToken);
        }

        private static async Task<IBlobEntityContainer<T>> CreateContainerForInternalAsync<T>(this IBlobEntityContainerFactory factory, Action<IBlobEntityContainerOptionsBuilder>? configure, PublicAccessType publicAccessType, CancellationToken cancellationToken)
            where T : class
        {
            var container = factory.GetContainerFor<T>(configure);
            await container.CreateIfNotExistsAsync(publicAccessType: publicAccessType, cancellationToken: cancellationToken).ConfigureAwait(false);
            return container;
        }
    }
}