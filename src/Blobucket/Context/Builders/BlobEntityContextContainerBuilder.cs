using System;
using System.Collections.Generic;
using Blobucket.Builders;
using Blobucket.Formatters;

namespace Blobucket.Context.Builders
{
    internal class BlobEntityContextContainerBuilder : IBlobEntityContextContainerBuilder
    {
        private readonly BlobEntityContextBuilder _builder;
        private readonly List<Action<IBlobEntityContainerOptionsBuilder>> _containerDelegates = new List<Action<IBlobEntityContainerOptionsBuilder>>();
        private readonly List<Action<string, IBlobEntityOptionsBuilder>> _entityDelegates = new List<Action<string, IBlobEntityOptionsBuilder>>();

        public BlobEntityContextContainerBuilder(BlobEntityContextBuilder builder)
            => _builder = builder ?? throw new ArgumentNullException(nameof(builder));

        public IBlobEntityContextContainerBuilder ConfigureContainerFor<T>()
            => _builder.ConfigureContainerFor<T>();

        public IBlobEntityContextEntityBuilder WhenEntity(Func<string, bool>? predicate = null)
        {
            var entityBuilder = new BlobEntityContextEntityBuilder(this);
            _entityDelegates.Add((id, builder) => 
            {
                if (predicate is null || predicate(id))
                {
                    entityBuilder.Build(builder);
                }
            });
            return entityBuilder;
        }

        public IBlobEntityContextContainerBuilder UseContainerName<T>()
        {
            _containerDelegates.Add(x => x.UseContainerName<T>());
            return this;
        }

        public IBlobEntityContextContainerBuilder UseContainerName(string containerName)
        {
            _containerDelegates.Add(x => x.UseContainerName(containerName));
            return this;
        }

        public IBlobEntityContextContainerBuilder UseFormatter<T>(T formatter) where T : BlobEntityFormatter
        {
            _containerDelegates.Add(x => x.UseFormatter(formatter));
            return this;
        }

        public void Build(IBlobEntityContainerOptionsBuilder builder)
        {
            foreach(var callback in _containerDelegates)
            {
                callback(builder);
            }
        }

        public void Build(string id, IBlobEntityOptionsBuilder builder)
        {
            foreach(var callback in _entityDelegates)
            {
                callback(id, builder);
            }
        }
    }
}
