using System;
using Azure.Storage.Blobs;
using Blobucket.Builders;
using Blobucket.Formatters;

namespace Blobucket
{
    public sealed class BlobEntityContainerFactory : IBlobEntityContainerFactory
    {
        private readonly BlobServiceClient _serviceClient;
        private readonly BlobEntityFormatter _formatter;

        public BlobEntityContainerFactory(string connectionString, BlobEntityFormatter formatter)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (formatter is null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            _serviceClient = new BlobServiceClient(connectionString);
            _formatter = formatter;
        }

        public IBlobEntityContainer<T> GetContainerFor<T>(Action<IBlobEntityContainerOptionsBuilder>? configure = null)
        {
            var builder = new BlobEntityContainerOptionsBuilder();

            builder
                .UseContainerName<T>()
                .UseFormatter(_formatter);

            var options = builder
                            .Append(configure)
                            .Build();

            return new BlobEntityContainer<T>(_serviceClient.GetBlobContainerClient(options.ContainerName), options);
        }
    }
}
