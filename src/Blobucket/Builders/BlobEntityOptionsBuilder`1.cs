using System;
using System.Collections.Generic;

namespace Blobucket.Builders
{
    internal abstract class ObjectBuilder<T>
        where T : new()
    {
        private readonly List<Action<T>> _delegates = new List<Action<T>>();

        protected void Add(Action<T> @delegate)
            => _delegates.Add(@delegate);

        public T Build()
        {
            var options = new T();

            foreach(var action in _delegates)
            {
                action(options);
            }

            return options;
        }
    }
}
