using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket.Formatters
{
    public class MetadataBlobEntityFormatterTests
    {
        [Fact]
        public async Task CanAddMetadataFromDictinoary()
        {
            var formatter = new MetadataBlobEntityFormatter(BlobEntityFormatter.Null, new Dictionary<string, string> { { "hello", "world" }});
            
            await formatter.Invoking(async x =>
                {
                    var metadata = new Dictionary<string, string>();
                    var stream = await x.SerializeAsync("data", metadata);
                    metadata.Count.Should().Be(1);
                    metadata.ContainsKey("hello").Should().BeTrue();
                    metadata["hello"].Should().Be("world");
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanAddMetadataDynamically()
        {
            var formatter = new MetadataBlobEntityFormatter(BlobEntityFormatter.Null, metadata => metadata["hello"] = "world");
            
            await formatter.Invoking(async x =>
                {
                    var metadata = new Dictionary<string, string>();
                    var stream = await x.SerializeAsync("data", metadata);
                    metadata.Count.Should().Be(1);
                    metadata.ContainsKey("hello").Should().BeTrue();
                    metadata["hello"].Should().Be("world");
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanAddMetadataDynamicallyWithEntity()
        {
            var formatter = new MetadataBlobEntityFormatter(BlobEntityFormatter.Null, (entity, metadata) => metadata["hello"] = (string)entity);
            
            await formatter.Invoking(async x =>
                {
                    var metadata = new Dictionary<string, string>();
                    var stream = await x.SerializeAsync("data", metadata);
                    metadata.Count.Should().Be(1);
                    metadata.ContainsKey("hello").Should().BeTrue();
                    metadata["hello"].Should().Be("data");
                }).Should().NotThrowAsync();
        }
    }
}