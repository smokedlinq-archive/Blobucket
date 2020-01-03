using System;
using System.Collections.Generic;
using System.Threading;
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
                    var stream = await x.SerializeAsync(new Model(), Mock.Of<IDictionary<string, string>>());
                    stream.Should().NotBeNull();
                    stream.Length.Should().BeGreaterThan(0);
                }).Should().NotThrowAsync();
        }

        [Fact]
        public async Task CanDeserialize()
        {
            var formatter = new CsvBlobEntityFormatter();
            var stream = await formatter.SerializeAsync(new Model(), Mock.Of<IDictionary<string, string>>());

            await formatter.Invoking(async x =>
                {
                    var model = await x.DeserializeAsync<Model>(stream, Mock.Of<IReadOnlyDictionary<string, string>>());
                    model.Should().NotBeNull();
                    model.StringProperty.Should().Be("string");
                    model.Int32Property.Should().Be(1);
                }).Should().NotThrowAsync();
        }

        class Model
        {
            public string StringProperty { get; set; } = "string";
            public int Int32Property { get; set; } = 1;
        }
    }
}