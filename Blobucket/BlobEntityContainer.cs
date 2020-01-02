using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _containerClient = (blobService ?? throw new ArgumentNullException(nameof(blobService))).GetBlobContainerClient(options.ContainerName);
            _defaultFormatter = options.Formatter;
        }

        public BlobEntityContainer(BlobContainerClient client, BlobEntityContainerOptions<T> options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _containerClient = client ?? throw new ArgumentNullException(nameof(client));
            _defaultFormatter = options.Formatter;
        }

        public string Name => _containerClient.Name;

        internal Task CreateIfNotExistsAsync(PublicAccessType publicAccessType = PublicAccessType.None, CancellationToken cancellationToken = default)
            => _containerClient.CreateIfNotExistsAsync(publicAccessType: publicAccessType, cancellationToken : cancellationToken);

        internal void CreateIfNotExists(PublicAccessType publicAccessType = PublicAccessType.None)
            => _containerClient.CreateIfNotExists(publicAccessType: publicAccessType);

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

            return await formatter.DeserializeAsync<T>(download.Value.Content, new ReadOnlyDictionary<string, string>(download.Value.Details.Metadata), cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task SetAsync(string id, T entity, BlobEntityOptions? options = null, bool overwrite = true, CancellationToken cancellationToken = default)
        {
            var formatter = options?.Formatter ?? _defaultFormatter;
            var client = _containerClient.GetBlobClient(id);
            var metadata = new Dictionary<string, string>();
            var conditions = overwrite ? null : new BlobRequestConditions { IfNoneMatch = new ETag("*") };
            using var stream = await formatter.SerializeAsync(entity, metadata, cancellationToken).ConfigureAwait(false);
            await client.UploadAsync(stream, metadata: metadata, conditions: conditions, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public Task DeleteAsync(string id, DeleteSnapshotsOption snapshotsOption = DeleteSnapshotsOption.None, CancellationToken cancellationToken = default)
            => _containerClient.GetBlobClient(id).DeleteIfExistsAsync(snapshotsOption: snapshotsOption, cancellationToken: cancellationToken);

        public Task CreateSnapshotAsync(string id, IDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
            => _containerClient.GetBlobClient(id).CreateSnapshotAsync(metadata: metadata, cancellationToken: cancellationToken);
    }
}
