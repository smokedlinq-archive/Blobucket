using System;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blobucket;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlobEntityContainerServiceCollectionExtensions
    {
        public static IServiceCollection AddBlobEntityContainerFactory(this IServiceCollection services, Action<BlobEntityContainerFactoryOptions>? configure = null, BlobServiceClient? serviceClient = null)
            => services.AddSingleton<BlobEntityContainerFactory>(_ => 
            {
                var options = new BlobEntityContainerFactoryOptions();
                configure?.Invoke(options);
                return new BlobEntityContainerFactory(options, serviceClient);
            });

        public static IServiceCollection AddBlobEntityContainer<T>(this IServiceCollection services, Action<BlobEntityContainerOptionsBuilder<T>>? configure = null, PublicAccessType publicAccessType = PublicAccessType.None)
            => services.AddSingleton<BlobEntityContainer<T>>(provider =>
                {
                    var container = provider.GetRequiredService<BlobEntityContainerFactory>().GetContainerFor<T>(configure);
                    container.CreateIfNotExists(publicAccessType);
                    return container;
                });
    }
}