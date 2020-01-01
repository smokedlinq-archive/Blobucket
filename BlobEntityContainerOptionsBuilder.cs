using System;
using System.Collections.Generic;
using System.Linq;
using Blobucket.Formatters;

namespace Blobucket
{
    public class BlobEntityContainerOptionsBuilder<T>
    {
        private readonly List<Action<BlobEntityContainerOptions<T>>> _configureDelegates = new List<Action<BlobEntityContainerOptions<T>>>();

        private BlobEntityContainerOptionsBuilder<T> Add(Action<BlobEntityContainerOptions<T>> @delegate)
        {
            _configureDelegates.Add(@delegate);
            return this;
        }

        public BlobEntityContainerOptionsBuilder<T> UseContainerName(string containerName)
            => Add(x => x.ContainerName = containerName);

        public BlobEntityContainerOptionsBuilder<T> UseFormatter<TFormatter>(TFormatter formatter)
            where TFormatter : BlobEntityFormatter, new()
            => Add(x => x.Formatter = formatter);

        public BlobEntityContainerOptions<T> Build()
        {
            var options = new BlobEntityContainerOptions<T>();

            foreach(var action in _configureDelegates)
            {
                action(options);
            }

            return options;
        }
    }
}
