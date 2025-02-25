using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess;
using MallardMessageHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Uno.Extensions;

namespace ApplicationTemplate;

/// <summary>
/// This class is used for API configuration.
/// - Configures API clients.
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
		// TODO: Configure your HTTP clients here.

		// For example purpose: the following line loads the DadJokesRepository configuration section and makes IOptions<DadJokesApiClientOptions> available for DI.
		services.BindOptionsToConfiguration<DadJokesApiClientOptions>(configuration);

		services
			.AddMainHandler()
			.AddNetworkExceptionHandler()
			.AddExceptionHubHandler()
			.AddAuthenticationTokenHandler()
			.AddTransient<HttpDebuggerHandler>()
			.AddResponseContentDeserializer()
			.AddAuthentication()
			.AddPosts(configuration)
			.AddUserProfile()
			.AddMinimumVersion()
			.AddKillSwitch()
			.AddDadJokes(configuration);

		return services;
	}

	private static IServiceCollection AddUserProfile(this IServiceCollection services)
	{
		// This one doesn't have an actual remote API yet. It's always a mock implementation.
		return services.AddSingleton<IUserProfileRepository, UserProfileRepositoryMock>();
	}

	private static IServiceCollection AddMinimumVersion(this IServiceCollection services)
	{
		// This one doesn't have an actual remote API yet. It's always a mock implementation.
		return services.AddSingleton<IMinimumVersionProvider, MinimumVersionProviderMock>();
	}

	private static IServiceCollection AddKillSwitch(this IServiceCollection services)
	{
		// This one doesn't have an actual remote API yet. It's always a mock implementation.
		return services.AddSingleton<IKillSwitchDataSource, KillSwitchDataSourceMock>();
	}

	private static IServiceCollection AddAuthentication(this IServiceCollection services)
	{
		// This one doesn't have an actual remote API yet. It's always a mock implementation.
		return services.AddSingleton<IAuthenticationApiClient, AuthenticationApiClientMock>();
	}

	private static IServiceCollection AddPosts(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.AddSingleton<IErrorResponseInterpreter<PostErrorResponse>>(s => new ErrorResponseInterpreter<PostErrorResponse>(
				(request, response, deserializedResponse) => deserializedResponse.Error != null,
				(request, response, deserializedResponse) => new PostRepositoryException(deserializedResponse)
			))
			.AddTransient<ExceptionInterpreterHandler<PostErrorResponse>>()
			.AddApiClient<IPostsRepository, PostsRepositoryMock>(configuration, "PostApiClient", b => b
				.AddHttpMessageHandler<ExceptionInterpreterHandler<PostErrorResponse>>()
				.AddHttpMessageHandler<AuthenticationTokenHandler<AuthenticationData>>()
			);
	}

	private static IServiceCollection AddDadJokes(this IServiceCollection services, IConfiguration configuration)
	{
		return services.AddApiClient<IDadJokesApiClient, DadJokesApiClientMock>(configuration, "DadJokesApiClient");
	}

	private static IServiceCollection AddApiClient<TInterface, TMock>(
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
			var options = configuration.GetSection(name).Get<ApiClientOptions>();
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
					new NetworkAvailabilityChecker(ct => Task.FromResult(s.GetRequiredService<IConnectivityProvider>().State is ConnectivityState.Internet))
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
		client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "DadJokesApp/1.0.0");
	}

	/// <summary>
	/// Adds a Refit client to the service collection.
	/// </summary>
	/// <typeparam name="T">The type of the Refit interface.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <param name="settingsProvider">Optional. The function to provide customized RefitSettings.</param>
	/// <returns>The updated IHttpClientBuilder.</returns>
	private static IHttpClientBuilder AddRefitHttpClient<T>(this IServiceCollection services, Func<IServiceProvider, RefitSettings> settingsProvider = null)
		where T : class
	{
		services.AddSingleton(serviceProvider =>
		{
			var settings = settingsProvider?.Invoke(serviceProvider) ?? new RefitSettings();
			settings.ContentSerializer = serviceProvider.GetRequiredService<IHttpContentSerializer>();
			return RequestBuilder.ForType<T>(settings);
		});

		return services
			.AddHttpClient(typeof(T).FullName)
			.AddTypedClient((client, serviceProvider) => RestService.For(client, serviceProvider.GetService<IRequestBuilder<T>>()));
	}
}
