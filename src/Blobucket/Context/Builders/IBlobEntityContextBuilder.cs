namespace Blobucket.Context.Builders
{
    public interface IBlobEntityContextBuilder
    {
        IBlobEntityContextContainerBuilder ConfigureContainerFor<T>();
    }
}