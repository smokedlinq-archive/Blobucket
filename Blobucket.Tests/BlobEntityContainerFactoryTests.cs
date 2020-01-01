using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Blobucket.Formatters;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket
{
    public class BlobEntityContainerFactoryTests
    {
        [Fact]
        public void GetContainerForDoesNotThrow()
        {
            var factory = new BlobEntityContainerFactory(
                            new BlobEntityContainerFactoryOptions 
                            { 
                                ConnectionString = @"UseDevelopmentStorage=true;" 
                            });

            factory.Invoking(x => x.GetContainerFor<string>().Should().NotBeNull()).Should().NotThrow();
        }

        [Fact]
        public void GetContainerForDoesNotThrowWhenUsingBlobServiceClient()
        {
            var options = new BlobEntityContainerFactoryOptions 
                            { 
                                ConnectionString = @"UseDevelopmentStorage=true;"
                            };
            var client = new BlobServiceClient(@"UseDevelopmentStorage=true;");
            var factory = new BlobEntityContainerFactory(options, serviceClient: client);

            factory.Invoking(x => x.GetContainerFor<string>()).Should().NotThrow();
        }
        
        [Fact]
        public void GetContainerForCanBeMocked()
        {
            var mock = new Mock<BlobEntityContainerFactory>(Mock.Of<BlobEntityContainerFactoryOptions>(), Mock.Of<BlobServiceClient>());

            mock.Setup(x => x.GetContainerFor<string>(It.IsAny<Action<BlobEntityContainerOptionsBuilder<string>>>()))
                .Returns(new Mock<BlobEntityContainer<string>>(Mock.Of<BlobServiceClient>(), Mock.Of<BlobEntityContainerOptions<string>>()).Object);
            
            mock.Setup(x => x.GetContainerFor<object>(It.IsAny<Action<BlobEntityContainerOptionsBuilder<object>>>()))
                .Returns(new Mock<BlobEntityContainer<object>>(Mock.Of<BlobServiceClient>(), Mock.Of<BlobEntityContainerOptions<object>>()).Object);
            
            var factory = mock.Object;

            factory.Invoking(x => 
                    x.GetContainerFor<string>().Should().NotBeNull())
                .Should().NotThrow();

            factory.Invoking(x => 
                    x.GetContainerFor<object>().Should().NotBeNull())
                .Should().NotThrow();
        }

        [Fact]
        public void GetContainerForUsesTheTypeNameToLowerCaseAsTheContainerName()
        {
            var factory = new BlobEntityContainerFactory(
                            new BlobEntityContainerFactoryOptions 
                            { 
                                ConnectionString = @"UseDevelopmentStorage=true;" 
                            });

            factory.Invoking(x => 
                    x.GetContainerFor<Array>().Name.Should().Be("array"))
                .Should().NotThrow();
        }

        [Fact]
        public void GetContainerForUsesTheTypeNameWithDashesSeparateWordsAsTheContainerName()
        {
            var factory = new BlobEntityContainerFactory(
                            new BlobEntityContainerFactoryOptions 
                            { 
                                ConnectionString = @"UseDevelopmentStorage=true;" 
                            });

            factory.Invoking(x => 
                    x.GetContainerFor<ConsoleColor>().Name.Should().Be("console-color"))
                .Should().NotThrow();
        }

        [Fact]
        public void GetContainerForWithCustomContainerName()
        {
            var factory = new BlobEntityContainerFactory(
                            new BlobEntityContainerFactoryOptions 
                            { 
                                ConnectionString = @"UseDevelopmentStorage=true;" 
                            });

            factory.Invoking(x => 
                    x.GetContainerFor<string>(c => c.UseContainerName("custom")).Name.Should().Be("custom"))
                .Should().NotThrow();
        }

        [Fact]
        public void GetContainerForWithCustomFormatter()
        {
            var factory = new BlobEntityContainerFactory(
                            new BlobEntityContainerFactoryOptions 
                            { 
                                ConnectionString = @"UseDevelopmentStorage=true;"
                            });

            factory.Invoking(x => x.GetContainerFor<string>(c => c.UseFormatter(Mock.Of<BlobEntityFormatter>()))).Should().NotThrow();
        }

        [Fact]
        public async Task CreateContainerForAsyncShouldNotThrow()
        {
            var mock = new Mock<BlobEntityContainerFactory>(Mock.Of<BlobEntityContainerFactoryOptions>(), Mock.Of<BlobServiceClient>());

            mock.Setup(x => x.GetContainerFor<string>(It.IsAny<Action<BlobEntityContainerOptionsBuilder<string>>>()))
                .Returns(new BlobEntityContainer<string>(Mock.Of<BlobContainerClient>(), Mock.Of<BlobEntityContainerOptions<string>>()));
            
            var factory = mock.Object;

            await factory.Invoking(async x => (await x.CreateContainerForAsync<string>()).Should().NotBeNull()).Should().NotThrowAsync();
        }
    }
}