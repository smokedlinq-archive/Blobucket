using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Blobucket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blobucketeer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .AddCommandLine(args)
                            .Build();

            var services = new ServiceCollection()
                            .AddBlobEntityContainerFactory(c => c.ConnectionString = config.GetConnectionString("Default") ?? "UseDevelopmentStorage=true;")
                            .AddBlobEntityContainer<Person>(c => c.UseContainerName("people"))
                            .BuildServiceProvider();
            
            var people = services.GetRequiredService<BlobEntityContainer<Person>>();

            // Create/update the blob
            await people.SetAsync("outlook.com/smokedlinq", new Person
                        {
                            FirstName = "First",
                            LastName = "Last",
                            EmailAddress = "smokedlinq@outlook.com"
                        }).ConfigureAwait(false);

            // Display the blob
            var me = await people.GetAsync("outlook.com/smokedlinq").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>").ConfigureAwait(false);

            // Create a snapshot before updating
            await people.CreateSnapshotAsync("outlook.com/smokedlinq").ConfigureAwait(false);

            // Update the blob
            await people.SetAsync("outlook.com/smokedlinq", new Person
                        {
                            FirstName = "Adam",
                            LastName = "Weigert",
                            EmailAddress = "smokedlinq@outlook.com"
                        }).ConfigureAwait(false);

            // Display the updated blob
            me = await people.GetAsync("outlook.com/smokedlinq").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>").ConfigureAwait(false);

            // Delete the blob and all snapshots
            await people.DeleteAsync("outlook.com/smokedlinq", DeleteSnapshotsOption.IncludeSnapshots).ConfigureAwait(false);
        }
    }

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
    }
}
