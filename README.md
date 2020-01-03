# Blobucket

[![Build Status](https://dev.azure.com/smokedlinq/Blobucket/_apis/build/status/smokedlinq.Blobucket?branchName=master)](https://dev.azure.com/smokedlinq/Blobucket/_build/latest?definitionId=5&branchName=master)
[![NuGet](https://img.shields.io/nuget/dt/Blobucket.svg)](https://www.nuget.org/packages/Blobucket)
[![NuGet](https://img.shields.io/nuget/vpre/Blobucket.svg)](https://www.nuget.org/packages/Blobucket)

The library is based on .NET Standard 2.1 and the Azure Storage v12 SDK. It enables an application to use an Azure storage account to persist objects/entities as blobs.

---

## Getting Started

Add the packages from the good ole nugets, e.g. using dotnet cli:

```batch
dotnet add package Blobucket
dotnet add package Blobucket.Formatters.Json
```

### Configuration

The library requires at a minimum a connection string to an Azure storage account and to choose a [formatter](#formatter). The Azure Storage Emulator is sufficient to get started, use `"UseDevelopmentServer=true;"` as the connection string. Below is more information on what else can be configured.

### Factory vs Context

There are two ways to use the library, the [`BlobEntityContainerFactory`](#BlobEntityContainerFactory) or the [`BlobEntityContext`](#BlobEntityContext) depending on how much control is needed. Factory is the core of the library. Context wraps the factory classes functionality with methods that do what the library is intended to do, read and write POCO blobs.

The factory can provide a richer dependency injection experience with a per entity container object. The context can be used for quick solutions.

*See the [Blobucketeer](samples/Blobucketeer/) sample project for the three ways the library was designed to be used.*

### BlobEntityContainerFactory

The `IBlobEntityContainerFactory` respresents access to the Azure storage account and is used for getting references to `IBlobEntityContainer` class. The factory can be instantiated with `BlobEntityContainerFactory` or by adding it to `IServiceCollection` with `AddBlobEntityContainerFactory` and then using the interface on your dependency injection.

```csharp
var factory = new BlobEntityContainerFactory("UseDevelopmentServer=true;", new JsonBlobEntityFormatter());
...
services.AddBlobEntityContainerFactory("UseDevelopmentServer=true;", new JsonBlobEntityFormatter());
```

#### BlobEntityContainer

The `IBlobEntityContainer<T>` represents the storage account container where all files accessed are of type `T`, though this doesn't mean you can only use one type per container, just that a type is targeted to a single container and you could have multiple types written to a single container. The factory is responsible for returning references to all containers, use the `GetContainerFor<T>` factory method or use the `IServiceCollection` extension method `AddBlobEntityContianer<T>` for dependency injection.

```csharp
var container = factory.GetContainerFor<Person>();
...
services.AddBlobEntityContainer<Person>();
```

##### Container Name

The default container name uses the type name of `T`, e.g. if `T` is `OrderDetails` then the container name would be `order-details`. If the type name does not meet the container naming convention for Azure storage accounts or if you want to use a custom container name, use the `UseContainerName` method during configuration.

```csharp
factory.GetContainerFor<Person>(builder => builder.UseContainerName("my-container-name"));
...
services.AddBlobEntityContainer<Person>(builder => builder.UseContainerName("my-container-name"));
```

##### Formatter

The container formatter is inherited from the factory and can be changed during instantiation with the `UseFormatter` method.

```csharp
factory.GetContainerFor<Person>(builder => builder.UseFormatter(new CsvBlobEntityFormatter()));
...
services.AddBlobEntityContainer<Person>(builder => builder.UseFormatter(new CsvBlobEntityFormatter()));
```

The formatter can also be different per entity when requested from the container.

```csharp
var entity = container.GetBlobEntity("id", builder => builder.UseFormatter(new CsvBlobEntityFormatter()));
```

#### BlobEntity

The `IBlobEntity<T>` represents a single blob. All actions for the blob are contained in this type.


#### Factory Example

```csharp
using System;
using System.Threading.Tasks;
using Blobucket;

public class Program
{
    public static async Task Main(string[] args)
    {
        var factory = new BlobEntityContainerFactory("UseDevelopmentStorage=true;", new JsonBlobEntityFormatter());

        var people = await factory.CreateContainerForAsync<Person>(config => config.UseContainerName("people"));

        var me = people.GetBlobEntity("outlook.com/smokedlinq");

        await me.SetAsync(new Person
                    {
                        FirstName = "Adam",
                        LastName = "Weigert",
                        EmailAddress = "smokedlinq@outlook.com"
                    });

        var me = await me.GetAsync();

        await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>");

        await me.DeleteAsync();
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

### BlobEntityContext

The `IBlobEntityContext` represents all actions that can be taken against blobs. The need for a factory, containers, and blob objects are handled by the class and it provides a simple fluent way to configure the containers and entities.

#### Context Example

```csharp
using System;
using System.Threading.Tasks;
using Blobucket.Context;

public class Program
{
    public static async Task Main(string[] args)
    {
        var context = new BlobEntityContext("UseDevelopmentStorage=true;", new JsonBlobEntityFormatter(), builder => builder
                        .ConfigureContainerFor<Person>()
                            .UseContainerName("people")
                            .WhenEntity(id => id.StartsWith("gmail.com/", StringComparison.CurrentCulture))
                                .UseFormatter(new CsvBlobEntityFormatter()));

        await context.SetAsync("outlook.com/smokedlinq", new Person
                    {
                        FirstName = "Adam",
                        LastName = "Weigert",
                        EmailAddress = "smokedlinq@outlook.com"
                    });

        var me = await context.GetAsync<Person>("outlook.com/smokedlinq");

        await Console.Out.WriteLineAsync($"{me.FirstName} {me.LastName}<{me.EmailAddress}>");

        await context.DeleteAsync<Person>("outlook.com/smokedlinq");
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

The `IBlobEntityContainerFactory`, `IBlobEntityContainer<T>`, and `IBlobEntityContext` classes can be registered with the Microsoft `IServiceCollection` during startup and used like any other registered type.

```csharp
services.AddBlobEntityContainerFactory("UseDevelopmentStorage=true;", new JsonBlobEntityFormatter())
        .AddBlobEntityContainer<Person>(c => c.UseContainerName("people"));
...
services.AddBlobEntityContext("UseDevelopmentStorage=true;", new JsonBlobEntityFormatter());
```

---

## Formatters

To create a custom formatter, inherit from the `BlobEntityFormatter` class and implement the abstract members. The stream is either downloaded from or uploaded to the Azure blob service.

The formatter should be thread-safe as it is created as a singleton during the factory and container operations, thus it should be stateless.

The `metadata` parameter on the methods allows during serialization to set the metadata on the blob. This allows for advanced scenarios where a piece of metadata could be used to use logic to determine which formatter to call based on metadata stored with the blob.

Current list of out of the box formatters:

- Blobucket.Formatters.Json
- Blobucket.Formatters.Csv (experimental)
