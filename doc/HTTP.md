## HTTP

We use [Microsoft.Extensions.Http](https://www.nuget.org/packages/Microsoft.Extensions.Http) for any HTTP related work.

- You can register a service with a dependency to `HttpClient` using `services.AddHttpClient<IEndpoint, EndpointImplementation>()`.

- We created a `NetworkAwareMessageHandler` which checks network connectivity if an exception is thrown when executing an HTTP request.
You can add this handler using `.AddHttpMessageHandler<NetworkAwareMessageHandler>()`.
We could create an extension `AddNoNetworkAware` which would auto-register a transient `NetworkAwareMessageHandler`.

- We could create more `DelegatingHandlers` to leverage more use cases (e.g. OAuth2.0, error handling, etc.). 

We use [JSONPlaceholder](https://jsonplaceholder.typicode.com/) as a testing API. 

- This API lets us send GET, POST, PUT, DELETE, etc. requests with proper mocked responses. 

- We currently use the `/posts` route which contains 100 posts.

We use [Refit](https://www.nuget.org/packages/Refit/) to generate the implementation of the client layer.

At the moment, we use [Xamarin.Essentials](https://www.nuget.org/packages/Xamarin.Essentials/) to test the network connectivity as it is not implemented in Uno.

We may use [Microsoft.Extensions.Http.Polly](https://www.nuget.org/packages/Microsoft.Extensions.Http.Polly/) to leverage request policies (e.g. retry, timeout, etc.).

### References
- [Making HTTP requests using IHttpClientFactory](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0)
- [Polly and HttpClientFactory](https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory)
- [What is Refit](https://github.com/reactiveui/refit)
- [Testing network connectivity using Xamarin.Essentials](https://docs.microsoft.com/en-us/xamarin/essentials/connectivity)
