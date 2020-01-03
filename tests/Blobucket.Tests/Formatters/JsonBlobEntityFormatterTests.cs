using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket.Formatters
{
    public class JsonBlobEntityFormatterTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var formatter = new JsonBlobEntityFormatter();
            
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
            var formatter = new JsonBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data", Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var data = await x.DeserializeAsync<string>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    data.Should().Be("data");
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenDeserializeFailsShouldThrowBlobEntityFormatterException()
        {
            var formatter = new JsonBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data", Mock.Of<IDictionary<string, string>>());
            await formatter.Invoking(async x => await x.DeserializeAsync<int>(stream, Mock.Of<IReadOnlyDictionary<string, string>>())).Should().ThrowAsync<BlobEntityFormatterException>();
        }

        [Fact]
        public async Task WhenSerializationIsCancelledShouldThrowOperationCanceledException()
        {
            var formatter = new JsonBlobEntityFormatter();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            await formatter.Invoking(async x => await x.SerializeAsync("data", Mock.Of<IDictionary<string, string>>(), cancellationToken: cts.Token)).Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task WhenDeserializationIsCancelledShouldThrowOperationCanceledException()
        {
            var formatter = new JsonBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data", Mock.Of<IDictionary<string, string>>());
            var cts = new CancellationTokenSource();
            cts.Cancel();
            await formatter.Invoking(async x => await x.DeserializeAsync<string>(stream, Mock.Of<IReadOnlyDictionary<string, string>>(), cancellationToken: cts.Token)).Should().ThrowAsync<OperationCanceledException>();
        }
    }
}