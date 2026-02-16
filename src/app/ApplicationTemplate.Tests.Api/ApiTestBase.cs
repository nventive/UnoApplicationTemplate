using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationTemplate.Tests;

/// <summary>
/// Gives access to the API endpoints and their configuration.
/// </summary>
public abstract class ApiTestBase : IDisposable
{
	private readonly IHost _host;

	protected ApiTestBase()
	{
		_host = new HostBuilder()
			.AddConfiguration(string.Empty, new TestEnvironmentManager())
			.ConfigureServices((context, s) => s
				.AddSingleton<TimeProvider>(TimeProvider.System)
				.AddDiagnostics(context.Configuration)
				.AddMock(context.Configuration)
				.AddApi(context.Configuration)
				.AddSerialization()
		).Build();
	}

	public void Dispose()
	{
		_host.Dispose();
	}

	/// <summary>
	/// Returns the requested service.
	/// </summary>
	/// <typeparam name="TService">Type of service.</typeparam>
	/// <returns>The requested service.</returns>
	protected virtual TService GetService<TService>()
	{
		return _host.Services.GetRequiredService<TService>();
	}
}
