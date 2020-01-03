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
    public class TransformBlobEntityFormatterTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var formatter = new TransformBlobEntityFormatter<string>(new BinaryBlobEntityFormatter(), convertTo: x => string.Join(';', (string[])x), convertFrom: x => ((string)x).Split(';'));
            
            await formatter.Invoking(async x =>
                {
                    var stream = await x.SerializeAsync(new string[] { "hello", "world" }, Mock.Of<IDictionary<string, string>>());
                    stream.Should().NotBeNull();
                    stream.Length.Should().BeGreaterThan(0);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeserialize()
        {
            var formatter = new TransformBlobEntityFormatter<string>(new BinaryBlobEntityFormatter(), convertTo: x => string.Join(';', (string[])x), convertFrom: x => ((string)x).Split(';'));
            var stream = await formatter.SerializeAsync(new string[] { "hello", "world" }, Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var entity = await x.DeserializeAsync<string[]>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    entity.Should().NotBeNull();
                    entity.Length.Should().Be(2);
                    entity[0].Should().Be("hello");
                    entity[1].Should().Be("world");
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task WhenConvertDelegatesThrowThenABlobEntityFormatterExceptionIsThrown()
        {
            var formatter = new TransformBlobEntityFormatter<string>(new BinaryBlobEntityFormatter(), convertTo: x => throw new Exception("convertTo failed"), convertFrom: x => throw new Exception("convertFrom failed"));

            await formatter.Invoking(async x =>
                {
                    await x.SerializeAsync<string>("data", Mock.Of<Dictionary<string, string>>());
                }).Should().ThrowAsync<Exception>();

            await formatter.Invoking(async x =>
                {
                    var stream = new MemoryStream(Convert.FromBase64String("AAEAAAD/////AQAAAAAAAAAGAQAAAAAL"));
                    await x.DeserializeAsync<string>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                }).Should().ThrowAsync<Exception>();
        }
    }
}