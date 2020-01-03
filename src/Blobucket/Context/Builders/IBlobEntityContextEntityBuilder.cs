using System;
using Blobucket.Formatters;

namespace Blobucket.Context.Builders
{
    public interface IBlobEntityContextEntityBuilder
    {
        IBlobEntityContextContainerBuilder ConfigureContainerFor<T>();
        IBlobEntityContextEntityBuilder WhenEntity(Func<string, bool> predicate);
        IBlobEntityContextEntityBuilder UseFormatter<T>(T formatter) 
            where T : BlobEntityFormatter;
    }
}
