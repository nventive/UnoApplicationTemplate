## Dependency injection

We use [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) for any IoC related work.

### Configuring

The host is configured inside the [Startup.cs](src/ApplicationTemplate.Shared/Startup.cs) file.

- **File configuration**: The configuration is loaded from the [appsettings.json](src/ApplicationTemplate.Shared/appsettings.json) file. We could have a settings file per configuration (e.g. _appsettings.production.json_).
We extract the file from the assembly and use `AddJsonStream` to import it. 
We could use `.AddJsonFile`, but it freezes in WebAssembly.
As of now, loading the configuration file doesn't create poor startup performance (< 500ms startup times). It will be something to check in the future.

- **In-memory configuration**: The configuration can also be loaded from a dictionnary using `AddInMemoryConfiguration`.

- **Precedence**: Multiple configurations can be loaded, the order in which they are loaded determines which one is used.
For example, if you have two keys with the same name, the last one will overwrite the first one.

- **Custom properties**: Custom properties can be added to the configuration. They can then be resolved using `ctx.Configuration.GetValue("MyKey")`.

### Registering

- Services are registered into `IServiceCollection`. We can register different services depending on the current configuration defined into the `HostBuilderContext.HostingEnvironment.EnvironmentName` property. 

- We could leverage more hosting extensions under the `Microsoft.Extensions.DependencyInjection` namespace when adding internal packages.
For example, we could have a `.AddLocation()` extension to include everything IoC related to a location services.

- You can register a singleton service using `services.AddSingleton<IService, ServiceImplementation>()`. This service will be shared by every user.

  - You don't need to specify any constructor parameters if all dependencies can be resolved automatically. Otherwise, you can use `services.AddSingleton<IService>(s => new ServiceImplementation(..))`.

- You can register a service that will be created everytime you request it by using `services.AddTransient<IService, ServiceImplementation>()`. 

- You can use `TryAdd*` which will register the service only if there hasn't been a registration of that type yet.

- You can't register named dependencies as this is [generally a bad practice](https://stackoverflow.com/questions/46476112/dependency-injection-of-multiple-instances-of-same-type-in-asp-net-core-2).

### Resolving

- Dependencies are injected automatically into the constructors.

- If you can't use constructor injection, you can use `IServiceProvider.GetRequiredService<IService>()` to resolve a service.
This will throw an exception if the type `IService` is not registered.
You can also use `IServiceProvider.GetService<IService>()` which will return `default(IService)` if the type is not registered.

- Circular dependencies will not work with this container. If you do have them, you will get the following exception `A circular dependency was detected for the service of type`.

### References

- [Understanding Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-3.0)
- [Using dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0)
