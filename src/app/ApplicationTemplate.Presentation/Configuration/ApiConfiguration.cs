using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using MallardMessageHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Uno.Extensions;

namespace ApplicationTemplate;

/// <summary>
/// This class is used for API configuration.
/// - Configures API endpoints.
/// - Configures HTTP handlers.
/// </summary>
public static class ApiConfiguration
{
	/// <summary>
	/// Adds the API services to the <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/>.</param>
	/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
	/// <returns>The updated <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
	{
		// For example purpose: the following line loads the DadJokesEndpoint configuration section and make IOptions<DadJokesEndpointOptions> available for DI.
		services.BindOptionsToConfiguration<DadJokesEndpointOptions>(configuration);

		services
			.AddMainHandler()
			.AddNetworkExceptionHandler()
			.AddExceptionHubHandler()
			.AddAuthenticationTokenHandler()
			.AddTransient<HttpDebuggerHandler>()
			.AddResponseContentDeserializer()
			.AddAuthenticationEndpoint()
			.AddPostEndpoint(configuration)
			.AddUserProfileEndpoint()
			.AddDadJokesEndpoint(configuration);

		return services;
	}

	private static IServiceCollection AddUserProfileEndpoint(this IServiceCollection services)
	{
		// This one doesn't have an actual remote endpoint yet. It's always a mock implementation.
		return services.AddSingleton<IUserProfileEndpoint, UserProfileEndpointMock>();
	}

	private static IServiceCollection AddAuthenticationEndpoint(this IServiceCollection services)
	{
		// This one doesn't have an actual remote endpoint yet. It's always a mock implementation.
		return services.AddSingleton<IAuthenticationEndpoint, AuthenticationEndpointMock>();
	}

	private static IServiceCollection AddPostEndpoint(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.AddSingleton<IErrorResponseInterpreter<PostErrorResponse>>(s => new ErrorResponseInterpreter<PostErrorResponse>(
				(request, response, deserializedResponse) => deserializedResponse.Error != null,
				(request, response, deserializedResponse) => new PostEndpointException(deserializedResponse)
			))
			.AddTransient<ExceptionInterpreterHandler<PostErrorResponse>>()
			.AddEndpoint<IPostEndpoint, PostEndpointMock>(configuration, "PostEndpoint", b => b
				.AddHttpMessageHandler<ExceptionInterpreterHandler<PostErrorResponse>>()
				.AddHttpMessageHandler<AuthenticationTokenHandler<AuthenticationData>>()
			);
	}

	private static IServiceCollection AddDadJokesEndpoint(this IServiceCollection services, IConfiguration configuration)
	{
		return services.AddEndpoint<IDadJokesEndpoint, DadJokesEndpointMock>(configuration, "DadJokesEndpoint");
	}

	private static IServiceCollection AddEndpoint<TInterface, TMock>(
		this IServiceCollection services,
		IConfiguration configuration,
		string name,
		Func<IHttpClientBuilder, IHttpClientBuilder> configure = null
	)
		where TInterface : class
		where TMock : class, TInterface
	{
		var mockOptions = configuration.GetSection("Mock").Get<MockOptions>();
		if (mockOptions.IsMockEnabled)
		{
			services.AddSingleton<TInterface, TMock>();
		}
		else
		{
			var options = configuration.GetSection(name).Get<EndpointOptions>();
			var diagnosticsOptions = configuration.ReadOptions<DiagnosticsOptions>();
			var httpClientBuilder = services
				.AddRefitHttpClient<TInterface>()
				.ConfigurePrimaryHttpMessageHandler(serviceProvider => serviceProvider.GetRequiredService<HttpMessageHandler>())
				.ConfigureHttpClient((serviceProvider, client) =>
				{
					client.BaseAddress = options.Url;
					AddDefaultHeaders(client, serviceProvider);
				})
				.AddConditionalHttpMessageHandler<HttpDebuggerHandler>(diagnosticsOptions.IsHttpDebuggerEnabled)
				.AddHttpMessageHandler<ExceptionHubHandler>();

			configure?.Invoke(httpClientBuilder);

			httpClientBuilder.AddHttpMessageHandler<NetworkExceptionHandler>();
		}

		return services;
	}

	private static IServiceCollection AddMainHandler(this IServiceCollection services)
	{
		return services.AddTransient<HttpMessageHandler, HttpClientHandler>();
	}

	private static IServiceCollection AddResponseContentDeserializer(this IServiceCollection services)
	{
		return services.AddSingleton<IResponseContentDeserializer, JsonSerializerToResponseContentSererializerAdapter>();
	}

	private static IServiceCollection AddNetworkExceptionHandler(this IServiceCollection services)
	{
		return services
			.AddSingleton<INetworkAvailabilityChecker>(s =>
					new NetworkAvailabilityChecker(ct => Task.FromResult(s.GetRequiredService<IConnectivityProvider>().NetworkAccess is NetworkAccess.Internet))
			)
			.AddTransient<NetworkExceptionHandler>();
	}

	private static IServiceCollection AddExceptionHubHandler(this IServiceCollection services)
	{
		return services
			.AddSingleton<IExceptionHub>(new ExceptionHub())
			.AddTransient<ExceptionHubHandler>();
	}

	private static IServiceCollection AddAuthenticationTokenHandler(this IServiceCollection services)
	{
		return services
			.AddSingleton<IAuthenticationTokenProvider<AuthenticationData>>(s => s.GetRequiredService<IAuthenticationService>())
			.AddTransient<AuthenticationTokenHandler<AuthenticationData>>();
	}

	private static void AddDefaultHeaders(HttpClient client, IServiceProvider serviceProvider)
	{
		client.DefaultRequestHeaders.Add("Accept-Language", CultureInfo.CurrentCulture.Name);

		// TODO #172779: Looks like our UserAgent is not of a valid format.
		// TODO #183437: Find alternative for UserAgent.
		// client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", serviceProvider.GetRequiredService<IEnvironmentService>().UserAgent);
	}

	/// <summary>
	/// Adds a Refit client to the service collection.
	/// </summary>
	/// <typeparam name="T">Type of the Refit interface</typeparam>
	/// <param name="services">Service collection</param>
	/// <param name="settings">Optional. Settings to configure the instance with</param>
	/// <returns>Updated IHttpClientBuilder</returns>
	private static IHttpClientBuilder AddRefitHttpClient<T>(this IServiceCollection services, Func<IServiceProvider, RefitSettings> settings = null)
		where T : class
	{
		services.AddSingleton(serviceProvider => RequestBuilder.ForType<T>(settings?.Invoke(serviceProvider)));

		return services
			.AddHttpClient(typeof(T).FullName)
			.AddTypedClient((client, serviceProvider) => RestService.For(client, serviceProvider.GetService<IRequestBuilder<T>>()));
	}
}
