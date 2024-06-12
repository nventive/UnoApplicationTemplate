using System;
using Microsoft.Extensions.Configuration;

namespace ApplicationTemplate;

/// <summary>
/// Implementation of the <see cref="IAppStoreUriProvider"/>.
/// </summary>
public sealed class AppStoreUriProvider : IAppStoreUriProvider
{
	private readonly IConfiguration _configuration;

	/// <summary>
	/// Initializes a new instance of the <see cref="AppStoreUriProvider"/> class.
	/// </summary>
	/// <param name="configuration"> The configuration where we get the Uris. </param>
	public AppStoreUriProvider(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	/// <inheritdoc />
	public Uri GetAppStoreUri()
	{
		var mockOptions = _configuration.GetSection("ApplicationStoreUrls").Get<ApplicationStoreUrisOptions>();
		Uri uri;

#if __ANDROID__
		uri = mockOptions.Android;
#elif __IOS__
		uri = mockOptions.Ios;
#else
		throw new NotImplementedException();
#endif
		return uri;
	}
}
