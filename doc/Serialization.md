## Serialization

We use [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json?view=net-6.0) for (de)serialization. More specifically, we use its [source generation features](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation?pivots=dotnet-6-0) for better performance.

- The serialization settings are configured inside the [SerializationConfiguration.cs](../src/app/ApplicationTemplate.Access/Configuration/SerializationConfiguration.cs) file.

  - This is where you will add your serializable types.
  - You'll find a few adapters in the [serialization folder](../src/app/ApplicationTemplate.Access/Framework/Serialization) to enable interop between different systems.
  - There are also converters to help serialization of custom types.