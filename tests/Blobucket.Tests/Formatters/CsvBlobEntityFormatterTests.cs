using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Blobucket.Formatters
{
    public class CsvBlobEntityFormatterTests
    {
        [Fact]
        public async Task CanSerialize()
        {
            var formatter = new CsvBlobEntityFormatter();
            
            await formatter.Invoking(async x =>
                {
                    var stream = await x.SerializeAsync(new Entity(), Mock.Of<IDictionary<string, string>>());
                    stream.Should().NotBeNull();
                    stream.Length.Should().BeGreaterThan(0);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeserialize()
        {
            var formatter = new CsvBlobEntityFormatter();
            var stream = await formatter.SerializeAsync(new Entity(), Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var model = await x.DeserializeAsync<Entity>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    model.Should().NotBeNull();
                    model.StringProperty.Should().Be("string");
                    model.Int32Property.Should().Be(1);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanManageCsvWithHeader()
        {
            var formatter = new CsvBlobEntityFormatter(hasHeader: true);
            var stream = await formatter.SerializeAsync(new Entity(), Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var model = await x.DeserializeAsync<Entity>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    model.Should().NotBeNull();
                    model.StringProperty.Should().Be("string");
                    model.Int32Property.Should().Be(1);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task SupportsIEnumerableT()
        {
            var formatter = new CsvBlobEntityFormatter(hasHeader: true);
            Stream stream = Stream.Null;
            
            await formatter.Invoking(async x => 
                {
                    stream = await x.SerializeAsync(new[] { new Entity(), new Entity() }.AsEnumerable(), Mock.Of<IDictionary<string, string>>());
                }).Should().NotThrowAsync();

            await formatter.Invoking(async x =>
                {
                    var entities = await x.DeserializeAsync<IEnumerable<Entity>>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    entities.Should().NotBeNull();
                    entities.Count().Should().Be(2);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task SupportsArray()
        {
            var formatter = new CsvBlobEntityFormatter(hasHeader: true);
            Stream stream = Stream.Null;
            
            await formatter.Invoking(async x => 
                {
                    stream = await x.SerializeAsync(new[] { new Entity(), new Entity() }, Mock.Of<IDictionary<string, string>>());
                }).Should().NotThrowAsync();

            await formatter.Invoking(async x =>
                {
                    var entities = await x.DeserializeAsync<Entity[]>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    entities.Should().NotBeNull();
                    entities.Length.Should().Be(2);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task SupportsIListT()
        {
            var formatter = new CsvBlobEntityFormatter(hasHeader: true);
            Stream stream = Stream.Null;
            
            await formatter.Invoking(async x => 
                {
                    stream = await x.SerializeAsync(new[] { new Entity(), new Entity() }.ToList(), Mock.Of<IDictionary<string, string>>());
                }).Should().NotThrowAsync();

            await formatter.Invoking(async x =>
                {
                    var entities = await x.DeserializeAsync<IList<Entity>>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    entities.Should().NotBeNull();
                    entities.Count.Should().Be(2);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeserializeUnsupportedEnumerableTypeThrowsBlobEntityFormatterException()
        {
            var formatter = new CsvBlobEntityFormatter(hasHeader: true);
            var stream = await formatter.SerializeAsync(new[] { new Entity(), new Entity() }, Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var entities = await x.DeserializeAsync<IDictionary<string, string>>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                }).Should().ThrowAsync<BlobEntityFormatterException>();
        }

        class Entity
        {
            public string StringProperty { get; set; } = "string";
            public int Int32Property { get; set; } = 1;
        }
    }
}