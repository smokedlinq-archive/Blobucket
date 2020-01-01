using System;
using Azure.Storage.Blobs;

namespace Blobucket
{
    public class BlobEntityContainerFactory
    {
        private readonly BlobServiceClient _serviceClient;

        public BlobEntityContainerFactory(BlobEntityContainerFactoryOptions options, BlobServiceClient? serviceClient = null)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _serviceClient = serviceClient ?? new BlobServiceClient(options.ConnectionString);
        }

        public virtual BlobEntityContainer<T> GetContainerFor<T>(Action<BlobEntityContainerOptionsBuilder<T>>? configure = null)
        {
            var options = BuildContainerOptionsFor<T>(configure);
            return new BlobEntityContainer<T>(_serviceClient, options);
        }

        protected virtual BlobEntityContainerOptions<T> BuildContainerOptionsFor<T>(Action<BlobEntityContainerOptionsBuilder<T>>? configure = null)
        {
            var builder = new BlobEntityContainerOptionsBuilder<T>();            
            configure?.Invoke(builder);
            return builder.Build();
        }
    }
}
