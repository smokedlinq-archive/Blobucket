using System;
using Blobucket.Context;
using Blobucket.Context.Builders;
using Blobucket.Formatters;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlobEntityContextServiceCollectionExtensions
    {
        public static IServiceCollection AddBlobEntityContext(this IServiceCollection services, string connectionString, BlobEntityFormatter formatter, Action<IBlobEntityContextBuilder>? configure = null)
        {
            var builder = new BlobEntityContextBuilder();
            configure?.Invoke(builder);
            services.AddSingleton<IBlobEntityContext>(new BlobEntityContext(connectionString, formatter, builder));
            return services;
        }
    }
}