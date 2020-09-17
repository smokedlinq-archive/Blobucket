using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable S5773

namespace Blobucket.Formatters
{
    public sealed class BinaryBlobEntityFormatter : BlobEntityFormatter
    {
        public override Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                return Task.FromResult((T)new BinaryFormatter().Deserialize(stream));
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }

        public override Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
            where T : class
        {
            try
            {
                Stream stream = new MemoryStream();
                new BinaryFormatter().Serialize(stream, entity);
                stream.Position = 0;
                return Task.FromResult(stream);
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }
    }
}