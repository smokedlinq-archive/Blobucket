using System;
using System.Collections.Generic;
using Blobucket.Builders;
using Blobucket.Formatters;

namespace Blobucket.Context.Builders
{
    internal class BlobEntityContextEntityBuilder : IBlobEntityContextEntityBuilder
    {
        private readonly BlobEntityContextContainerBuilder _context;
        private readonly List<Action<IBlobEntityOptionsBuilder>> _delegates = new List<Action<IBlobEntityOptionsBuilder>>();

        public BlobEntityContextEntityBuilder(BlobEntityContextContainerBuilder context)
            => _context = context;

        public IBlobEntityContextEntityBuilder UseFormatter<T>(T formatter)
            where T : BlobEntityFormatter
        {
            _delegates.Add(x => x.UseFormatter(formatter));
            return this;
        }

        public IBlobEntityContextContainerBuilder ConfigureContainerFor<T>()
            => _context.ConfigureContainerFor<T>();

        public IBlobEntityContextEntityBuilder WhenEntity(Func<string, bool> predicate)
            => _context.WhenEntity(predicate);

        public void Build(IBlobEntityOptionsBuilder builder)
        {
            foreach(var callback in _delegates)
            {
                callback(builder);
            }
        }
    }
}
