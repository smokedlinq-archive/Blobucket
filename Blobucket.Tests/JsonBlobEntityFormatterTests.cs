using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Blobucket.Formatters;
using FluentAssertions;
using Xunit;

namespace Blobucket
{
    public class JsonBlobEntityFormatterTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var formatter = new JsonBlobEntityFormatter();

            await formatter.Invoking(async x =>
                {
                    var stream = await x.SerializeAsync("data");
                    stream.Should().NotBeNull();
                    stream.Length.Should().BeGreaterThan(0);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeserialize()
        {
            var formatter = new JsonBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data");

            await formatter.Invoking(async x =>
                {
                    var data = await x.DeserializeAsync<string>(stream);
                    data.Should().Be("data");
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenDeserializeFailsShouldThrowBlobEntityFormatException()
        {
            var formatter = new JsonBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data");
            await formatter.Invoking(async x => await x.DeserializeAsync<int>(stream)).Should().ThrowAsync<BlobEntityFormatterException>();
        }

        [Fact]
        public async Task WhenSerializationIsCancelledShouldThrowOperationCanceledException()
        {
            var formatter = new JsonBlobEntityFormatter();
            var cts = new CancellationTokenSource();
            cts.Cancel();
            await formatter.Invoking(async x => await x.SerializeAsync("data", cancellationToken: cts.Token)).Should().ThrowAsync<OperationCanceledException>();
        }

        [Fact]
        public async Task WhenDeserializationIsCancelledShouldThrowOperationCanceledException()
        {
            var formatter = new JsonBlobEntityFormatter();
            var stream = await formatter.SerializeAsync("data");
            var cts = new CancellationTokenSource();
            cts.Cancel();
            await formatter.Invoking(async x => await x.DeserializeAsync<string>(stream, cancellationToken: cts.Token)).Should().ThrowAsync<OperationCanceledException>();
        }
    }
}