﻿using System;
using System.Threading.Tasks;
using Blobucket;

namespace Blobucketeer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var factory = new BlobEntityContainerFactory(new BlobEntityContainerFactoryOptions { ConnectionString = @"UseDevelopmentStorage=true;" });
            var people = await factory.CreateContainerForAsync<Person>(config => config.UseContainerName("people"));

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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
    }
}