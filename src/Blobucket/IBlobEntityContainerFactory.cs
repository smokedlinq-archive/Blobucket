using System;
using Blobucket.Builders;

namespace Blobucket
{
    public interface IBlobEntityContainerFactory
    {
        IBlobEntityContainer<T> GetContainerFor<T>(Action<IBlobEntityContainerOptionsBuilder>? configure = null)
            where T : class;
    }
}