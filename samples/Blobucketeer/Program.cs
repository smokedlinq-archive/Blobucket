using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Blobucketeer
{
    static partial class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                                .AddCommandLine(args)
                                .Build();

            await Console.Out.WriteLineAsync("UseFactory:\n----------------------------------------").ConfigureAwait(false);
            await UseFactory(config).ConfigureAwait(false);

            await Console.Out.WriteLineAsync("\nUseContext:\n----------------------------------------").ConfigureAwait(false);
            await UseContext(config).ConfigureAwait(false);

            await Console.Out.WriteLineAsync("\nUseCustomContext:\n----------------------------------------").ConfigureAwait(false);
            await UseCustomContext(config).ConfigureAwait(false);

            await Console.Out.WriteLineAsync("\nUseCsvFormatter:\n----------------------------------------").ConfigureAwait(false);
            await UseCsvFormatter(config).ConfigureAwait(false);
        }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
    }
}
