using System;
using System.Collections.Generic;
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
        static async Task UseCsvFormatter(IConfiguration config)
        {
            // Register the context for dependency injection
            var services = new ServiceCollection()
                            .AddBlobEntityContext(config.GetConnectionString("Default") ?? "UseDevelopmentStorage=true;", new CsvBlobEntityFormatter(hasHeader: true), builder =>
                                // register the 3 supported enumerable types to the same container
                                builder
                                    .ConfigureContainerFor<Person[]>()
                                        .UseContainerName("people")
                                    .ConfigureContainerFor<IEnumerable<Person>>()
                                        .UseContainerName("people")
                                    .ConfigureContainerFor<IList<Person>>()
                                        .UseContainerName("people")
                            )
                            .BuildServiceProvider();
            
            var context = services.GetRequiredService<IBlobEntityContext>();

            // Write multiple entities to the csv
            await context.SetAsync("outlook.com", new[]
                        {
                            new Person
                            {
                                FirstName = "Adam",
                                LastName = "Weigert",
                                EmailAddress = "smokedlinq@outlook.com"
                            },
                            new Person
                            {
                                FirstName = "Adam",
                                LastName = "Weigert",
                                EmailAddress = "adam.weigert@outlook.com"
                            }
                        }).ConfigureAwait(false);

            async Task Display(IEnumerable<Person> people)
            {
                foreach (var person in people)
                {
                    await Console.Out.WriteLineAsync($"  {person.FirstName} {person.LastName}<{person.EmailAddress}>").ConfigureAwait(false);
                }
            }

            // Read as an array
            var array = await context.GetAsync<Person[]>("outlook.com").ConfigureAwait(false);
            await Console.Out.WriteLineAsync("Person[]").ConfigureAwait(false);
            await Display(array).ConfigureAwait(false);

            // Read as an IList<Person>
            var list = await context.GetAsync<IList<Person>>("outlook.com").ConfigureAwait(false);
            await Console.Out.WriteLineAsync("\nIList<Person>").ConfigureAwait(false);
            await Display(list).ConfigureAwait(false);

            // Read as an IEnumerable<Person>
            var enumerable = await context.GetAsync<IEnumerable<Person>>("outlook.com").ConfigureAwait(false);
            await Console.Out.WriteLineAsync("\nIEnumerable<Person>").ConfigureAwait(false);
            await Display(enumerable).ConfigureAwait(false);

            // Delete the blob and all snapshots
            await context.DeleteAsync<Person>("outlook.com", DeleteSnapshotsOption.IncludeSnapshots).ConfigureAwait(false);
        }
    }
}
