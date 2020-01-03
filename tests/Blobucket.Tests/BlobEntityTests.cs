using System;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Blobucket.Formatters;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket
{
    public class BlobEntityTests
    {
        [Fact]
        public async Task GetAsyncDoesNotThrowAndReturnsNullWhenBlobNotFound()
        {
            var mockBlob = new Mock<BlobClient>();
            mockBlob
                .Setup(x => x.DownloadAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException((int)HttpStatusCode.NotFound, "not found"));

            var entity = new BlobEntity<string>(mockBlob.Object, new BlobEntityOptions());

            await entity.Invoking(async x => (await x.GetAsync()).Should().BeNull())
                        .Should()
                        .NotThrowAsync();
        }
        [Fact]
        public async Task GetAsyncDoesNotThrowAndReturnsNullWhenBlobIsZeroBytes()
        {
            var mockBlob = new Mock<BlobClient>();
            mockBlob
                .Setup(x => x.DownloadAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException((int)HttpStatusCode.RequestedRangeNotSatisfiable, "invalid range"));

            var entity = new BlobEntity<string>(mockBlob.Object, new BlobEntityOptions());

            await entity.Invoking(async x => (await x.GetAsync()).Should().BeNull())
                        .Should()
                        .NotThrowAsync();
        }

        [Fact]
        public async Task CanSetAsync()
        {
            var entity = new BlobEntity<string>(Mock.Of<BlobClient>(), new BlobEntityOptions());
            await entity.Invoking(x => x.SetAsync("data")).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeleteAsync()
        {
            var entity = new BlobEntity<string>(Mock.Of<BlobClient>(), new BlobEntityOptions());
            await entity.Invoking(x => x.DeleteAsync()).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanCreateSnapshotAsync()
        {
            var entity = new BlobEntity<string>(Mock.Of<BlobClient>(), new BlobEntityOptions());
            await entity.Invoking(x => x.CreateSnapshotAsync()).Should().NotThrowAsync();
        }
    }
}