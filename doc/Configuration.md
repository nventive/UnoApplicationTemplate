# Configuration

We use [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) for any configuration related work.

For more documentation on configuration, read the references listed at the bottom.

## Configuring

The host is configured inside the [Startup.cs](../src/app/ApplicationTemplate.Shared/Startup.cs) file.

- **File configuration**: The configuration is loaded from the [appsettings.json](../src/app/ApplicationTemplate.Shared/appsettings.json) file. We could have a settings file per configuration (e.g. _appsettings.production.json_).
We extract the file from the assembly and use `AddJsonStream` to import it. 
We could use `.AddJsonFile`, but it freezes in WebAssembly.
As of now, loading the configuration file doesn't create poor startup performance (< 500ms startup times). It will be something to check in the future.

- **In-memory configuration**: The configuration can also be loaded from a dictionnary using `AddInMemoryConfiguration`.

- **Precedence**: Multiple configurations can be loaded, the order in which they are loaded determines which one is used.
For example, if you have two keys with the same name, the last one will overwrite the first one.

## Accessing

- Custom properties can be added to the configuration. They can then be resolved using `IConfiguration`. 

  Assuming this is the content of your `appsettings.json`.
  ```json
  {
    "MySection": {
      "MyProperty1": "MyProperty1Value",
      "MyProperty2": {
        "MyProperty3": "MyProperty3Value"
      }
    }
  }
  ```

  There are multiple ways to resolve the properties.
  ```csharp
  // You can resolve the properties using IConfiguration as a dictionary.
  var property1 = configuration["MySection:MyProperty1"];

  // You can resolve the properties using strongly-typed Options.
  var property1 = configuration.GetSection("MySection").Get<MyOptions>();

  public class MyOptions
  {
      public string MyProperty1 { get; set; }

      public MyOtherOptions MyProperty2 { get; set; }

      ...
  }
  ```

- Configuration sections can be loaded and registred in DI as `IOptions<T>`. 

  - The easy way (to map a root section using pattern matching):
    1. Create a `MyConfigurationSectionOptions` object that map your configuration section named `MyConfigurationSection`
    1. Register the object

     ```csharp
     // Loads the MyConfigurationSection configuration section and make IOptions<MyConfigurationSectionOptions> available for DI.
    services.BindOptionsToConfiguration<MyConfigurationSectionOptions>(configuration);
    ```
  - The complete way (to map subsections or where class name does not match the `[sectionName]Options` pattern)
    1. Create a `MySubOrCustomSectionOptions` object that map your configuration section
    1. Register the object

     ```csharp
     // Loads the MySection:MySubSection configuration section and make IOptions<MySubOrCustomSectionOptions> available for DI.
    services.BindOptionsToConfiguration<MySubOrCustomSectionOptions>(configuration, "MySection:MySubSection");
    ```

- The `IConfiguration` interface is registered as a service to simplify the process of resolving an application setting.

  ```csharp
  // You can resolve the configuration in the constructor of a service using the IoC.
  public class MyService(IConfiguration configuration) { ... }

  // You can resolve the configuration from a view model using the IoC.
  var configuration = this.GetService<IConfiguration>();
  ```

## References

- [Using IConfiguration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)