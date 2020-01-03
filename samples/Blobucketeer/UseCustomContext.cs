using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket.Context;
using Blobucket.Formatters;
using Microsoft.Extensions.Configuration;

namespace Blobucketeer
{
    static partial class Program
    {
        class CustomContext : BlobEntityContext
        {
            public CustomContext(IConfiguration config)
                : base(config.GetConnectionString("Default") ?? "UseDevelopmentStorage=true;", new JsonBlobEntityFormatter())
            {
                // Configure the Person container to be 'people'
                ConfigureContainerFor<Person>()
                    .UseContainerName("people")
                    .WhenEntity();
            }
        }

        static async Task UseCustomContext(IConfiguration config)
        {
            // Skipping dependency injection, no out of the box support with a custom context
            var context = new CustomContext(config);

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
