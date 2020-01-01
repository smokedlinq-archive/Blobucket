using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Blobucket
{
    public class BlobServiceContext
    {
        private readonly BlobServiceClient _serviceClient;

        public BlobServiceContext(BlobServiceContextOptions options)
        {
            _serviceClient = new BlobServiceClient(options.ConnectionString);
        }

        public virtual async Task<IBlobEntityContainer<T>> CreateContainerForAsync<T>(Action<BlobEntityContainerOptionsBuilder<T>>? configure = null, CancellationToken cancellationToken = default)
        {
            var builder = new BlobEntityContainerOptionsBuilder<T>();
            
            configure?.Invoke(builder);

            var options = builder.Build();

            var container = new BlobEntityContainer<T>(_serviceClient, options);

            await container.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

            return container;
        }
    }
}
