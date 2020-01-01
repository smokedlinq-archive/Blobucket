using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityContainer<T>
    {
        private readonly BlobContainerClient _containerClient;
        private readonly BlobEntityFormatter _defaultFormatter;

        public BlobEntityContainer(BlobServiceClient blobService, BlobEntityContainerOptions<T> options)
        {
            _containerClient = (blobService ?? throw new ArgumentNullException(nameof(blobService))).GetBlobContainerClient(options.ContainerName);
            _defaultFormatter = (options ?? throw new ArgumentNullException(nameof(options))).Formatter;
        }

        public BlobEntityContainer(BlobContainerClient client, BlobEntityContainerOptions<T> options)
        {
            _containerClient = client ?? throw new ArgumentNullException(nameof(client));
            _defaultFormatter = (options ?? throw new ArgumentNullException(nameof(options))).Formatter;
        }

        public string Name => _containerClient.Name;

        internal Task CreateIfNotExistsAsync(CancellationToken cancellationToken = default)
            => _containerClient.CreateIfNotExistsAsync(cancellationToken : cancellationToken);

        public virtual async Task<T> GetAsync(string id, BlobEntityOptions? options = null, CancellationToken cancellationToken = default)
        {
            var formatter = options?.Formatter ?? _defaultFormatter;
            var client = _containerClient.GetBlobClient(id);
            Response<BlobDownloadInfo> download;

            try
            {
                download = await client.DownloadAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex)
            {
                if (ex.Status == (int)HttpStatusCode.NotFound)
                {
                    # nullable disable
                    return default;
                    # nullable enable
                }

                throw;
            }

            return await formatter.DeserializeAsync<T>(download.Value.Content).ConfigureAwait(false);
        }

        public async Task SetAsync(string id, T entity, BlobEntityOptions? options = null, bool overwrite = true, CancellationToken cancellationToken = default)
        {
            var formatter = options?.Formatter ?? _defaultFormatter;
            var client = _containerClient.GetBlobClient(id);
            using var stream = await formatter.SerializeAsync(entity, cancellationToken).ConfigureAwait(false);
            await client.UploadAsync(stream, overwrite: overwrite, cancellationToken);
        }

        public Task DeleteAsync(string id, BlobEntityOptions? options = null, CancellationToken cancellationToken = default)
            => _containerClient.GetBlobClient(id).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
