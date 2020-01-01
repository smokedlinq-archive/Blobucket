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
    public class BlobEntityContainerTests
    {
        [Fact]
        public async Task GetCanBeMocked()
        {
            var mock = new Mock<BlobEntityContainer<string>>(Mock.Of<BlobContainerClient>(), Mock.Of<BlobEntityContainerOptions<string>>());

            mock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<BlobEntityOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("data");

            var container = mock.Object;

            await container.Invoking(async x =>
                {
                    var data = await x.GetAsync("id");
                    data.Should().Be("data");
                }).Should().NotThrowAsync();
        }
        
        [Fact]
        public async Task GetReturnsNullWhenBlobNotFound()
        {
            var mockBlob = new Mock<BlobClient>();
            mockBlob
                .Setup(x => x.DownloadAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new RequestFailedException((int)HttpStatusCode.NotFound, "not found"));

            var mockContainer = new Mock<BlobContainerClient>();
            mockContainer.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlob.Object);
            
            var container = new BlobEntityContainer<string>(mockContainer.Object, new BlobEntityContainerOptions<string>());
            await container.Invoking(async x => (await x.GetAsync("id")).Should().BeNull()).Should().NotThrowAsync();
        }

        [Fact]
        public async Task SetDoesNotThrow()
        {
            var mockBlob = new Mock<BlobClient>();
            var mockContainer = new Mock<BlobContainerClient>();
            mockContainer.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlob.Object);
            
            var container = new BlobEntityContainer<string>(mockContainer.Object, new BlobEntityContainerOptions<string>());
            await container.Invoking(async x => await x.SetAsync("id", "data")).Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeleteDoesNotThrow()
        {
            var mockBlob = new Mock<BlobClient>();                
            var mockContainer = new Mock<BlobContainerClient>();
            mockContainer.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(mockBlob.Object);
            
            var container = new BlobEntityContainer<string>(mockContainer.Object, new BlobEntityContainerOptions<string>());
            await container.Invoking(async x => await x.DeleteAsync("id")).Should().NotThrowAsync();
        }
    }
}