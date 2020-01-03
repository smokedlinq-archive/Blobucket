using Blobucket.Formatters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blobucket
{
    public class BlobEntityContainerServiceCollectionExtensionsTests
    {
        [Fact]
        public void CanResolveContainerFromServiceProvider()
        {
            var services = new ServiceCollection()
                            .AddBlobEntityContainerFactory("UseDevelopmentStorage=true;", BlobEntityFormatter.Null)
                            .AddBlobEntityContainer<string>(c => c.UseContainerName("people"), createIfNotExists: false)
                            .BuildServiceProvider();
            
            services.Invoking(x => services.GetRequiredService<IBlobEntityContainer<string>>().Should().NotBeNull()).Should().NotThrow();
        }
    }
}