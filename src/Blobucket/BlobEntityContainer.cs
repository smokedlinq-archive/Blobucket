using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blobucket.Builders;
using Blobucket.Formatters;

namespace Blobucket
{
    public sealed class BlobEntityContainer<T> : IBlobEntityContainer<T>
        where T : class
    {
        private readonly BlobContainerClient _client;
        private readonly BlobEntityFormatter _formatter;

        public BlobEntityContainer(BlobContainerClient client, BlobEntityContainerOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _formatter = options.Formatter;
        }

        public Task CreateIfNotExistsAsync(PublicAccessType publicAccessType = PublicAccessType.None, CancellationToken cancellationToken = default)
            => _client.CreateIfNotExistsAsync(publicAccessType: publicAccessType, cancellationToken : cancellationToken);

        public IBlobEntity<T> GetBlobEntity(string id, Action<IBlobEntityOptionsBuilder>? configure = null)
        {
            var builder = new BlobEntityOptionsBuilder();

            builder.UseFormatter(_formatter);
            
            var options = builder
                            .Append(configure)
                            .Build();

            return new BlobEntity<T>(_client.GetBlobClient(id), options);
        }
    }
}
