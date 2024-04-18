using System;

namespace ApplicationTemplate;

/// <summary>
/// Provides the Uri to the app store for the current platform.
/// </summary>
public interface IAppStoreUriProvider
{
	/// <summary>
	/// Gets the Uri to the app store for the current platform.
	/// </summary>
	/// <returns>The Uri to the app store for the current platform.</returns>
	Uri GetAppStoreUri();
}
