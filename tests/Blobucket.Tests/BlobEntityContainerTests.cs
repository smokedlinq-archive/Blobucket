using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Blobucket.Formatters;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket
{
    public class BlobEntityContainerTests
    {
        [Fact]
        public void CanGetBlobEntity()
        {
            var mock = new Mock<BlobContainerClient>();
            mock.Setup(x => x.GetBlobClient(It.IsAny<string>())).Returns(Mock.Of<BlobClient>());
            var container = new BlobEntityContainer<string>(mock.Object, new BlobEntityContainerOptions());
            container.Invoking(x => x.GetBlobEntity("id").Should().NotBeNull()).Should().NotThrow();
        }
    }
}