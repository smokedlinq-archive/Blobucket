using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket;
using Blobucket.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blobucketeer
{
    static partial class Program
    {
        static async Task UseFactory(IConfiguration config)
        {
            // Register the factory and container for dependency injection
            var services = new ServiceCollection()
                            .AddBlobEntityContainerFactory(config.GetConnectionString("Default") ?? "UseDevelopmentStorage=true;", new JsonBlobEntityFormatter())
                            .AddBlobEntityContainer<Person>(c => c.UseContainerName("people"))
                            .BuildServiceProvider();
            
            var people = services.GetRequiredService<IBlobEntityContainer<Person>>();
            var blob = people.GetBlobEntity("outlook.com/smokedlinq");
            
            // Create/update the blob
            await blob.SetAsync(new Person
                        {
                            FirstName = "First",
                            LastName = "Last",
                            EmailAddress = "smokedlinq@outlook.com"
                        }).ConfigureAwait(false);

            // Display the blob
            var me = await blob.GetAsync().ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>").ConfigureAwait(false);

            // Create a snapshot before updating
            await blob.CreateSnapshotAsync().ConfigureAwait(false);

            // Update the blob
            await blob.SetAsync(new Person
                        {
                            FirstName = "Adam",
                            LastName = "Weigert",
                            EmailAddress = "smokedlinq@outlook.com"
                        }).ConfigureAwait(false);

            // Display the updated blob
            me = await blob.GetAsync().ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>").ConfigureAwait(false);

            // Delete the blob and all snapshots
            await blob.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots).ConfigureAwait(false);
        }
    }
}
