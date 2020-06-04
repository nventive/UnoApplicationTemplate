## Serialization

We use [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) for any serialization related work.

- **Sync serialization**: `string result = JsonSerializer.Serialize(myObject)`

- **Sync deserialization**: `MyType result = JsonSerializer.Deserialize<MyType>(myString)`

- **Async serialization**: `await JsonSerializer.SerializeAsync(dataStream, myObject, ct);`

- **Async deserialization**: `MyType result = await JsonSerializer.DeserializeAsync<MyType>(dataStream, ct);`

At the moment, this doesn't work on WebAssembly; there is an [issue with Uno.SourceGenerationTasks](https://github.com/unoplatform/Uno.SourceGeneration/issues/114).

### References
- [Using System.Text.Json](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/)
- [Adding System.Text.Json to HttpClientFactory](https://josefottosson.se/you-are-probably-still-using-httpclient-wrong-and-it-is-destabilizing-your-software/#withihttpclientfactory)
