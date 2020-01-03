using System;
using Azure.Storage.Blobs.Models;
using Blobucket;
using Blobucket.Builders;
using Blobucket.Formatters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlobEntityServiceCollectionExtensions
    {
        public static IServiceCollection AddBlobEntityContainerFactory(this IServiceCollection services, string connectionString, BlobEntityFormatter formatter)
            => services.AddSingleton<IBlobEntityContainerFactory>(new BlobEntityContainerFactory(connectionString, formatter));

        public static IServiceCollection AddBlobEntityContainer<T>(this IServiceCollection services, Action<IBlobEntityContainerOptionsBuilder>? configure = null, PublicAccessType publicAccessType = PublicAccessType.None, bool createIfNotExists = true)
            where T : class
            => services.AddSingleton<IBlobEntityContainer<T>>(provider =>
                {
                    var container = provider.GetRequiredService<IBlobEntityContainerFactory>().GetContainerFor<T>(configure);
                    if (createIfNotExists)
                    {
                        container.CreateIfNotExistsAsync(publicAccessType).Wait();
                    }
                    return container;
                });
    }
}