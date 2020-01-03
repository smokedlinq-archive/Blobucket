using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Blobucket.Formatters
{
    public class TransformBlobEntityFormatter<TTarget> : BlobEntityFormatter
        where TTarget : class
    {
        private readonly BlobEntityFormatter _formatter;
        private readonly Func<object, TTarget> _convertTo;
        private readonly Func<TTarget, object> _convertFrom;

        public TransformBlobEntityFormatter(BlobEntityFormatter formatter, Func<object, TTarget> convertTo, Func<TTarget, object> convertFrom)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _convertTo = convertTo ?? throw new ArgumentNullException(nameof(convertTo));
            _convertFrom = convertFrom ?? throw new ArgumentNullException(nameof(convertFrom));
        }

        public override async Task<T> DeserializeAsync<T>(Stream stream, IReadOnlyDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _formatter.DeserializeAsync<TTarget>(stream, metadata, cancellationToken).ConfigureAwait(false);
                return (T)_convertFrom(entity);
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }

        public override Task<Stream> SerializeAsync<T>(T entity, IDictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            try
            {
                var obj = _convertTo(entity);
                return _formatter.SerializeAsync(obj, metadata, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BlobEntityFormatterException(ex.Message, ex);
            }
        }
    }
}