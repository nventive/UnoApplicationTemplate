# HTTP

We use [Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http) for any HTTP related work.

For more documentation on HTTP requests, read the references listed at the bottom.

## HTTP clients

- You can register a service with a dependency to `HttpClient` using `services.AddHttpClient<IRepository, RepositoryImplementation>()` in the [ApiConfiguration.cs](../src/app/ApplicationTemplate.Presentation/Configuration/ApiConfiguration.cs) file.

- We use `DelegatingHandler` to create HTTP request / response pipelines. There are lot of delegating handlers implementation in the community, we provide some in [MallardMessageHandlers](https://github.com/nventive/MallardMessageHandlers).

- We use [Refit](https://www.nuget.org/packages/Refit/) to generate the HTTP implementations in the data access layer.

## Mocking

- We use a simple `BaseMock` class to support mocking scenarios. You simply add embbedded resources (.json files) that contain the mocked responses into your project.

## References
- [Making HTTP requests using IHttpClientFactory](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0)
- [Delegating handlers](https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/http-message-handlers)
- [Polly and HttpClientFactory](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory)
- [What is Refit](https://github.com/reactiveui/refit)
