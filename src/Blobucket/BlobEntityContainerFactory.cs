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
            _ = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _serviceClient = new BlobServiceClient(connectionString);
        }

        public IBlobEntityContainer<T> GetContainerFor<T>(Action<IBlobEntityContainerOptionsBuilder>? configure = null)
            where T : class
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
