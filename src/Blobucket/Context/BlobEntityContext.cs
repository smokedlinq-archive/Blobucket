using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket.Context.Builders;
using Blobucket.Formatters;

namespace Blobucket.Context
{
    public class BlobEntityContext : IBlobEntityContext
    {
        private readonly IBlobEntityContainerFactory _factory;
        private readonly ConcurrentDictionary<Type, object> _containers = new ConcurrentDictionary<Type, object>();
        private readonly BlobEntityContextBuilder _builder = new BlobEntityContextBuilder();
        
        public BlobEntityContext(string connectionString, BlobEntityFormatter formatter)
            => _factory = new BlobEntityContainerFactory(connectionString, formatter);

        public BlobEntityContext(IBlobEntityContainerFactory factory)
            => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

        internal BlobEntityContext(string connectionString, BlobEntityFormatter formatter, BlobEntityContextBuilder builder)
        {
            _factory = new BlobEntityContainerFactory(connectionString, formatter);
            _builder = builder;
        }

        public async Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync<T>(cancellationToken).ConfigureAwait(false);
            return await container.GetBlobEntity(id, x => _builder.Build<T>(id, x)).GetAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task SetAsync<T>(string id, T entity, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync<T>(cancellationToken).ConfigureAwait(false);
            await container.GetBlobEntity(id, x => _builder.Build<T>(id, x)).SetAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(string id, DeleteSnapshotsOption snapshotOption = DeleteSnapshotsOption.None, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync<T>(cancellationToken).ConfigureAwait(false);
            await container.GetBlobEntity(id).DeleteAsync(snapshotsOption: snapshotOption, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task CreateSnapshotAsync<T>(string id, IDictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
        {
            var container = await GetContainerAsync<T>(cancellationToken).ConfigureAwait(false);
            await container.GetBlobEntity(id).CreateSnapshotAsync(metadata: metadata, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private async Task<IBlobEntityContainer<T>> GetContainerAsync<T>(CancellationToken cancellationToken)
        {
            if (!_containers.TryGetValue(typeof(T), out var container))
            {
                container = await _factory.CreateContainerForAsync<T>(configure: x => _builder.Build<T>(x), cancellationToken: cancellationToken).ConfigureAwait(false);
                _containers.TryAdd(typeof(T), container);
            }

            return (IBlobEntityContainer<T>)container;
        }

        protected IBlobEntityContextContainerBuilder ConfigureContainerFor<T>()
            => _builder.ConfigureContainerFor<T>();
    }
}
