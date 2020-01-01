using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blobucket.Formatters;

namespace Blobucket
{
    internal class BlobEntityContainer<T> : IBlobEntityContainer<T>
    {
        private readonly BlobContainerClient _containerClient;
        private readonly BlobEntityFormatter _defaultFormatter;

        public BlobEntityContainer(BlobServiceClient blobService, BlobEntityContainerOptions<T> options)
        {
            _containerClient = blobService.GetBlobContainerClient(options.ContainerName);
            _defaultFormatter = options.Formatter;
        }

        internal Task CreateIfNotExistsAsync(CancellationToken cancellationToken = default)
            => _containerClient.CreateIfNotExistsAsync(cancellationToken : cancellationToken);

        public async Task<T> GetAsync(string id, BlobEntityOptions? options, CancellationToken cancellationToken = default)
        {
            var formatter = options?.Formatter ?? _defaultFormatter;
            var client = _containerClient.GetBlobClient(id);
            Response<BlobDownloadInfo> download;

            try
            {
                download = await client.DownloadAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new BlobEntityNotFoundException(ex.Message, ex);
            }

            return await formatter.DeserializeAsync<T>(download.Value.Content).ConfigureAwait(false);
        }

        public async Task SetAsync(string id, T entity, BlobEntityOptions? options = null, CancellationToken cancellationToken = default)
        {
            var formatter = options?.Formatter ?? _defaultFormatter;
            var client = _containerClient.GetBlobClient(id);
            using var stream = await formatter.SerializeAsync(entity, cancellationToken).ConfigureAwait(false);
            await client.UploadAsync(stream, cancellationToken);
        }

        public Task DeleteAsync(string id, BlobEntityOptions? options = null, CancellationToken cancellationToken = default)
            => _containerClient.GetBlobClient(id).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
