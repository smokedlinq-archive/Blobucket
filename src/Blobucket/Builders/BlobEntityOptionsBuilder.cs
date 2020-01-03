using System;
using Blobucket.Formatters;

namespace Blobucket.Builders
{
    internal class BlobEntityOptionsBuilder : ObjectBuilder<BlobEntityOptions>, IBlobEntityOptionsBuilder
    {
        public IBlobEntityOptionsBuilder UseFormatter<T>(T formatter) where T : BlobEntityFormatter
        {
            Add(x => x.Formatter = formatter);
            return this;
        }

        public BlobEntityOptionsBuilder Append(Action<IBlobEntityOptionsBuilder>? action)
        {
            action?.Invoke(this);
            return this;
        }
    }
}
