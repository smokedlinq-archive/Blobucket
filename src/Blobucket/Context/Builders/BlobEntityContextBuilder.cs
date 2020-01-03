using System;
using System.Collections.Generic;
using Blobucket.Builders;

namespace Blobucket.Context.Builders
{
    internal class BlobEntityContextBuilder : IBlobEntityContextBuilder
    {
        private readonly Dictionary<Type, BlobEntityContextContainerBuilder> _containerBuilders = new Dictionary<Type, BlobEntityContextContainerBuilder>();

        public IBlobEntityContextContainerBuilder ConfigureContainerFor<T>()
        {
            if (!_containerBuilders.TryGetValue(typeof(T), out var containerBuilder))
            {
                containerBuilder = new BlobEntityContextContainerBuilder(this);
                _containerBuilders.TryAdd(typeof(T), containerBuilder);
            }

            return containerBuilder;
        }
        
        public void Build<T>(IBlobEntityContainerOptionsBuilder builder)
        {
            if (_containerBuilders.TryGetValue(typeof(T), out var contextBuilder))
            {
                contextBuilder.Build(builder);
            }
        }

        public void Build<T>(string id, IBlobEntityOptionsBuilder builder)
        {
            if (_containerBuilders.TryGetValue(typeof(T), out var contextBuilder))
            {
                contextBuilder.Build(id, builder);
            }
        }
    }
}