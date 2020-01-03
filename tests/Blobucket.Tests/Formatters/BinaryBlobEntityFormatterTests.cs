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
    public class BinaryBlobEntityFormatterTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var formatter = new BinaryBlobEntityFormatter();
            
            await formatter.Invoking(async x =>
                {
                    var stream = await x.SerializeAsync("data", Mock.Of<IDictionary<string, string>>());
                    stream.Should().NotBeNull();
                    stream.Length.Should().BeGreaterThan(0);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeserialize()
        {
            var formatter = new BinaryBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data", Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var entity = await x.DeserializeAsync<string>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    entity.Should().NotBeNull();
                    entity.Should().Be("data");
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenTheTypeIsNotSerializableThenBlobEntityFormatterExceptionIsThrown()
        {
            var formatter = new BinaryBlobEntityFormatter();

            await formatter.Invoking(async x =>
                {
                    await x.SerializeAsync(new Entity(), Mock.Of<Dictionary<string, string>>());
                }).Should().ThrowAsync<Exception>();
        }

        class Entity
        {
        }
    }
}