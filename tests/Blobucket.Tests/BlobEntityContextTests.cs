using System;
using System.Threading;
using System.Threading.Tasks;
using Blobucket.Builders;
using Blobucket.Formatters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Blobucket.Context
{
    public class BlobEntityContextTests
    {
        [Fact]
        public void CanResolveContextFromServiceProvider()
        {
            var services = new ServiceCollection()
                            .AddBlobEntityContext("UseDevelopmentStorage=true;", BlobEntityFormatter.Null, builder =>
                                builder
                                    .ConfigureContainerFor<string>()
                                        .WhenEntity()
                                    .ConfigureContainerFor<IBlobEntityContainerFactory>()
                                        .UseContainerName<BlobEntityContainerFactory>())
                            .BuildServiceProvider();
            
            services.Invoking(x => services.GetRequiredService<IBlobEntityContext>().Should().NotBeNull()).Should().NotThrow();
        }

        [Fact]
        public async Task CanGetAsync()
        {
            var mockEntity = new Mock<IBlobEntity<string>>();
            mockEntity
                .Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync("data");

            var mockContainer = new Mock<IBlobEntityContainer<string>>();
            mockContainer
                .Setup(x => x.GetBlobEntity(It.IsAny<string>(), It.IsAny<Action<IBlobEntityOptionsBuilder>>()))
                .Callback<string, Action<IBlobEntityOptionsBuilder>>((_, configure) =>
                {
                    var builder = new BlobEntityOptionsBuilder();
                    configure?.Invoke(builder);
                })
                .Returns(mockEntity.Object);

            var mockFactory = new Mock<IBlobEntityContainerFactory>();
            mockFactory
                .Setup(x => x.GetContainerFor<string>(It.IsAny<Action<IBlobEntityContainerOptionsBuilder>>()))
                .Callback<Action<IBlobEntityContainerOptionsBuilder>>(configure =>
                {
                    var builder = new BlobEntityContainerOptionsBuilder();
                    configure?.Invoke(builder);
                })
                .Returns(mockContainer.Object);

            var context = new CustomContextWithFluentContainerAndEntityConfiguration(mockFactory.Object);

            var data = await context.GetAsync<string>("id");

            data.Should().Be("data");
        }

        [Fact]
        public async Task CanSetAsync()
        {
            var mockContainer = new Mock<IBlobEntityContainer<string>>();
            mockContainer
                .Setup(x => x.GetBlobEntity(It.IsAny<string>(), It.IsAny<Action<IBlobEntityOptionsBuilder>>()))
                .Returns(Mock.Of<IBlobEntity<string>>());

            var mockFactory = new Mock<IBlobEntityContainerFactory>();
            mockFactory
                .Setup(x => x.GetContainerFor<string>(It.IsAny<Action<IBlobEntityContainerOptionsBuilder>>()))
                .Returns(mockContainer.Object);

            var context = new CustomContextWithFluentContainerAndEntityConfiguration(mockFactory.Object);

            await context.Invoking(x => x.SetAsync<string>("id", "data")).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeleteAsync()
        {
            var mockContainer = new Mock<IBlobEntityContainer<string>>();
            mockContainer
                .Setup(x => x.GetBlobEntity(It.IsAny<string>(), It.IsAny<Action<IBlobEntityOptionsBuilder>>()))
                .Returns(Mock.Of<IBlobEntity<string>>());

            var mockFactory = new Mock<IBlobEntityContainerFactory>();
            mockFactory
                .Setup(x => x.GetContainerFor<string>(It.IsAny<Action<IBlobEntityContainerOptionsBuilder>>()))
                .Returns(mockContainer.Object);

            var context = new CustomContextWithFluentContainerAndEntityConfiguration(mockFactory.Object);

            await context.Invoking(x => x.DeleteAsync<string>("id")).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanCreateSnapshotAsync()
        {
            var mockContainer = new Mock<IBlobEntityContainer<string>>();
            mockContainer
                .Setup(x => x.GetBlobEntity(It.IsAny<string>(), It.IsAny<Action<IBlobEntityOptionsBuilder>>()))
                .Returns(Mock.Of<IBlobEntity<string>>());

            var mockFactory = new Mock<IBlobEntityContainerFactory>();
            mockFactory
                .Setup(x => x.GetContainerFor<string>(It.IsAny<Action<IBlobEntityContainerOptionsBuilder>>()))
                .Returns(mockContainer.Object);

            var context = new CustomContextWithFluentContainerAndEntityConfiguration(mockFactory.Object);

            await context.Invoking(x => x.CreateSnapshotAsync<string>("id")).Should().NotThrowAsync();
        }

        class CustomContextWithFluentContainerAndEntityConfiguration : BlobEntityContext
        {
            public CustomContextWithFluentContainerAndEntityConfiguration(IBlobEntityContainerFactory factory) : base(factory)
            {
                ConfigureContainerFor<string>()
                    .UseContainerName("things")
                    .UseFormatter(BlobEntityFormatter.Null)
                    .WhenEntity(id => id == "id")
                        .UseFormatter(BlobEntityFormatter.Null)
                    .WhenEntity(id => id != "id");
            }
        }
    }
}