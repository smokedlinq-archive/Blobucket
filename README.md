# Blobucket

[![Build Status](https://dev.azure.com/smokedlinq/Blobucket/_apis/build/status/smokedlinq.Blobucket?branchName=master)](https://dev.azure.com/smokedlinq/Blobucket/_build/latest?definitionId=5&branchName=master)
[![NuGet](https://img.shields.io/nuget/dt/Blobucket.svg)](https://www.nuget.org/packages/Blobucket)
[![NuGet](https://img.shields.io/nuget/vpre/Blobucket.svg)](https://www.nuget.org/packages/Blobucket)

The library is based on .NET Standard 2.1 and the Azure Storage v12 SDK. It enables an application to use an Azure storage account to persist objects/entities as blobs.

---

## Getting Started

Add the package from the good ole nugets, e.g. using dotnet cli:

```dotnet
dotnet add package Blobucket
```

### Configuration

The library requires at a minimum a connection string to an Azure storage account. The Azure Storage Emulator is sufficient to get started, use `"UseDevelopmentServer=true;"` as the connection string. Below is more information on what else can be configured.

#### BlobEntityContainerFactory

The `BlobEntityContainerFactory` respresents access to the Azure storage account. The factory can be configured using the `BlobEntityContainerFactoryOptions` class via the constructor or the `AddBlobEntityContainerFactory` depenedency injection extension method.

Configure the `ConnectionString` property; if this is not explicitly set the application will fail to connect to an Azure storage account.

```csharp
options.ConnectionString = "UseDevelopmentServer=true;";
```

#### BlobEntityContainer

The `BlobEntityContainer<T>` class represents a container within the Azure storage account where all files accessed are of type `T`. The container can be configured using the `BlobEntityContainerOptionsBuilder<T>` class via the constructor or the `AddBlobEntityContainer<T>` dependency injection extension method.

##### Container Name

The default container name uses the type name of `T`, e.g. if `T` is `OrderDetails` then the container name would be `order-details`. If the type name does not meet the container naming convention for Azure storage accounts or if you want to use a custom container name, use the `UseContainerName` method during configuration.

```csharp
builder.UseContainerName("my-container-name");
```

##### Formatter

The default formatter is the `JsonBlobEntityFormatter` that uses the `System.Text.Json` package to serialize the objects to JSON. The formatter can be configured at the factory, container, and entity, each inheriting from the previous.

To change the formatter for the factory use the `Formatter` property on the `BlobEntityContainerFactoryOptions` class.
```csharp
options.Formatter = new CsvBlobEntityFormatter();
```

To change the formatter for the container use the `UseFormatter` method on the `BlobEntityContainerOptionsBuilder<T>` class.

```csharp
builder.UseFormatter(new CsvBlobEntityFormatter());
```

The formatter can also be different per entity using the `BlobEntityOptions` on the `GetAsync` and `SetAsync` methods of the `BlobEntityContainer<T>` class.

*Make sure if the entity was created with a different formatter than the container uses that it is read with a compatible formatter.*

```csharp
var options = new BlobEntityOptions { Formatter = new CsvBlobEntityFormatter() };
await container.SetAsync("id", myObject, options);
await container.GetAsync("id", options);
```

### Example

Fire up the Azure Storage Emulator and create a new console project, replace the `Program.cs` file with below.

>A more advanced, end-to-end, sample can be found in the [Sample](Sample) folder.

```csharp
using System;
using System.Threading.Tasks;
using Blobucket;

public class Program
{
    public static async Task Main(string[] args)
    {
        var factory = new BlobEntityContainerFactory(
                        new BlobEntityContainerFactoryOptions 
                        {
                            ConnectionString = @"UseDevelopmentStorage=true;" 
                        });

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

---

## Dependency Injection

The `BlobEntityContainerFactory` and `BlobEntityContainer<T>` classes can be registered with the Microsoft `IServiceCollection` during startup and used like any other registered type.

```csharp
services.AddBlobEntityContainerFactory(c => c.ConnectionString = "UseDevelopmentStorage=true;")
        .AddBlobEntityContainer<Person>(c => c.UseContainerName("people"));
```

---

## Formatters

To create a custom formatter, inherit from the `BlobEntityFormatter` class and implement the abstract members. The stream is either downloaded from or uploaded to the Azure blob service.

The formatter should be thread-safe as it is created as a singleton during the factory and container operations, thus it should be stateless.

The `metadata` parameter on the methods allows during serialization to set the metadata on the blob. This allows for advanced scenarios where a piece of metadata could be used to use logic to determine which formatter to call based on metadata stored with the blob.
