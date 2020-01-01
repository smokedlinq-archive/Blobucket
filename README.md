Blobucket
=======
[![Build Status](https://dev.azure.com/smokedlinq/Blobucket/_apis/build/status/smokedlinq.Blobucket?branchName=master)](https://dev.azure.com/smokedlinq/Blobucket/_build/latest?definitionId=5&branchName=master)
[![NuGet](https://img.shields.io/nuget/dt/Blobucket.svg)](https://www.nuget.org/packages/Blobucket)
[![NuGet](https://img.shields.io/nuget/vpre/Blobucket.svg)](https://www.nuget.org/packages/Blobucket)

The library is based on .NET Standard 2.1 and the Azure Storage v12 SDK. It enables an application to use an Azure storage account to persist objects/entities as blobs.

## Getting Started

Add the package from the good ole nugets, e.g. using dotnet cli:

```dotnet
dotnet add package Blobucket
```

## Example Usage

Fire up the Azure Storage Emulator and then use `dotnet run` on this bad boy:

```csharp
using System;
using System.Threading.Tasks;
using Blobucket;

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
```
