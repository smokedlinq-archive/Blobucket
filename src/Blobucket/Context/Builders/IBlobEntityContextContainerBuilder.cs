using System;
using Blobucket.Formatters;

namespace Blobucket.Context.Builders
{
    public interface IBlobEntityContextContainerBuilder
    {
        IBlobEntityContextContainerBuilder ConfigureContainerFor<T>();
        IBlobEntityContextEntityBuilder WhenEntity(Func<string, bool>? predicate = null);
        IBlobEntityContextContainerBuilder UseContainerName<T>();
        IBlobEntityContextContainerBuilder UseContainerName(string containerName);
        IBlobEntityContextContainerBuilder UseFormatter<T>(T formatter)
            where T : BlobEntityFormatter;
    }
}
