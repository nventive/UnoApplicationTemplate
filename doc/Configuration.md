# Configuration

We use [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) for any configuration related work.

For more documentation on configuration, read the references listed at the bottom.

## Configuring

The `IConfiguration` is populated using 3 layers of configuration files.
1. `appsettings.json` is the base layer. Use this to set default values.
1. `appsettings.{environment}.json` is the environment specific layer. Use this to set values that are specific to environments such as API clients, API keys, etc. You can customize the environments like you want. Check [Environments.md](Environments.md) for more info.
   
   **Don't put production secrets directly in appsettings.production.json!** Instead, use a placeholder token along with a powershell replace in the  pipeline to inject secrets in your configuration files.
 
1. `appsettings.override.json` is an optional file that appears when you override a value from the application.

Check the `AddAppSettings` method from [AppSettingsConfiguration.cs](../src/app/ApplicationTemplate.Presentation/Configuration/AppSettingsConfiguration.cs) file to see how the 3 layers are setup.

## Accessing

- The `IConfiguration` interface is registered as a service to simplify the process of resolving an application setting.

  ```csharp
  // You can resolve the configuration in the constructor of a service using the IoC.
  public class MyService(IConfiguration configuration) { ... }

  // You can resolve the configuration from a view model using the IoC.
  var configuration = this.GetService<IConfiguration>();
  ```

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

## References

- [Using IConfiguration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1)