using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket.Context;
using Blobucket.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blobucketeer
{
    static partial class Program
    {
        static async Task UseContext(IConfiguration config)
        {
            // Register the context for dependency injection
            var services = new ServiceCollection()
                            .AddBlobEntityContext(config.GetConnectionString("Default") ?? "UseDevelopmentStorage=true;", new JsonBlobEntityFormatter(), builder =>
                                builder.ConfigureContainerFor<Person>()
                                        .UseContainerName("people")
                            )
                            .BuildServiceProvider();
            
            var context = services.GetRequiredService<IBlobEntityContext>();

            // Create/update the blob
            await context.SetAsync("outlook.com/smokedlinq", new Person
                        {
                            FirstName = "First",
                            LastName = "Last",
                            EmailAddress = "smokedlinq@outlook.com"
                        }).ConfigureAwait(false);

            // Display the blob
            var me = await context.GetAsync<Person>("outlook.com/smokedlinq").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>").ConfigureAwait(false);

            // Create a snapshot before updating
            await context.CreateSnapshotAsync<Person>("outlook.com/smokedlinq").ConfigureAwait(false);

            // Update the blob
            await context.SetAsync("outlook.com/smokedlinq", new Person
                        {
                            FirstName = "Adam",
                            LastName = "Weigert",
                            EmailAddress = "smokedlinq@outlook.com"
                        }).ConfigureAwait(false);

            // Display the updated blob
            me = await context.GetAsync<Person>("outlook.com/smokedlinq").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>").ConfigureAwait(false);

            // Delete the blob and all snapshots
            await context.DeleteAsync<Person>("outlook.com/smokedlinq", DeleteSnapshotsOption.IncludeSnapshots).ConfigureAwait(false);
        }
    }
}
