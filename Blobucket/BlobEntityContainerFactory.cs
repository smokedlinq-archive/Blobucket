using System;
using Azure.Storage.Blobs;
using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityContainerFactory
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly BlobEntityFormatter _formatter;

        public BlobEntityContainerFactory(BlobEntityContainerFactoryOptions options, BlobServiceClient? serviceClient = null)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _serviceClient = serviceClient ?? new BlobServiceClient(options.ConnectionString);
            _formatter = options.Formatter;
        }

        public virtual BlobEntityContainer<T> GetContainerFor<T>(Action<BlobEntityContainerOptionsBuilder<T>>? configure = null)
        {
            var options = BuildContainerOptionsFor<T>(configure);
            return new BlobEntityContainer<T>(_serviceClient, options);
        }

        protected virtual BlobEntityContainerOptions<T> BuildContainerOptionsFor<T>(Action<BlobEntityContainerOptionsBuilder<T>>? configure = null)
        {
            var builder = new BlobEntityContainerOptionsBuilder<T>();
            builder.UseFormatter(_formatter);
            configure?.Invoke(builder);
            return builder.Build();
        }
    }
}
