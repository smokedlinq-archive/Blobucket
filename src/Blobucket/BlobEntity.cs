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
    public sealed class BlobEntity<T> : IBlobEntity<T>
    {
        private readonly BlobClient _client;
        private readonly BlobEntityFormatter _formatter;

        public BlobEntity(BlobClient client, BlobEntityOptions options)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _client = client ?? throw new ArgumentNullException(nameof(client));
            _formatter = options.Formatter;
        }

        public async Task<T> GetAsync(CancellationToken cancellationToken = default)
        {
            Response<BlobDownloadInfo> download;

            try
            {
                download = await _client.DownloadAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (RequestFailedException ex)
            {
                // If the blob is not found (http response == 404)
                //  or it is a 0-byte blob (http response == 416)
                if (ex.Status == (int)HttpStatusCode.NotFound
                    || ex.Status == (int)HttpStatusCode.RequestedRangeNotSatisfiable)
                {
                    # nullable disable
                    return default;
                    # nullable enable
                }

                throw;
            }

            return await _formatter.DeserializeAsync<T>(download.Value.Content, new ReadOnlyDictionary<string, string>(download.Value.Details.Metadata), cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task SetAsync(T entity, bool overwrite = true, CancellationToken cancellationToken = default)
        {
            var metadata = new Dictionary<string, string>();
            var conditions = overwrite ? null : new BlobRequestConditions { IfNoneMatch = new ETag("*") };
            using var stream = await _formatter.SerializeAsync(entity, metadata, cancellationToken).ConfigureAwait(false);
            await _client.UploadAsync(stream, metadata: metadata, conditions: conditions, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public Task DeleteAsync(DeleteSnapshotsOption snapshotsOption = DeleteSnapshotsOption.None, CancellationToken cancellationToken = default)
            => _client.DeleteIfExistsAsync(snapshotsOption: snapshotsOption, cancellationToken: cancellationToken);

        public Task CreateSnapshotAsync(IDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
            => _client.CreateSnapshotAsync(metadata: metadata, cancellationToken: cancellationToken);
    }
}
