using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Blobucket
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                            .AddUserSecrets<Program>()
                            .AddEnvironmentVariables()
                            .AddCommandLine(args)
                            .Build();

            var context = new BlobServiceContext(new BlobServiceContextOptions { ConnectionString = config.GetConnectionString("Default") });
            var people = await context.CreateContainerForAsync<Person>(config => config.UseContainerName("people"));

            await people.SetAsync("outlook.com/smokedlinq", new Person
                        {
                            FirstName = "Adam",
                            LastName = "Weigert",
                            EmailAddress = "smokedlinq@outlook.com"
                        });

            var me = await people.GetAsync("outlook.com/smokedlinq");

            await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>");

            await people.DeleteAsync("outlook.com/smokedlinq");
        }
    }

    public class Person
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
    }
}
