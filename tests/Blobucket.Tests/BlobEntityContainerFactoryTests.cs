using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Blobucket.Builders;
using Blobucket.Formatters;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket
{
    public class BlobEntityContainerFactoryTests
    {
        [Fact]
        public void CanGetContainerFor()
        {
            var factory = new BlobEntityContainerFactory("UseDevelopmentStorage=true;", BlobEntityFormatter.Null);
            factory.Invoking(x => x.GetContainerFor<string>().Should().NotBeNull()).Should().NotThrow();
        }

        [Fact]
        public async Task CanCreateContainerForAsync()
        {
            var mock = new Mock<IBlobEntityContainerFactory>();
            mock.Setup(x => x.GetContainerFor<string>(It.IsAny<Action<IBlobEntityContainerOptionsBuilder>>()))
                .Returns(new BlobEntityContainer<string>(Mock.Of<BlobContainerClient>(), new BlobEntityContainerOptions()));
            
            var factory = mock.Object;
            var container = await factory.CreateContainerForAsync<string>();
            container.Should().NotBeNull();
        }
    }
}