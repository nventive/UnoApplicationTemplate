using System;

namespace ApplicationTemplate;

/// <summary>
/// Contains the configuration for the application store URLs.
/// </summary>
public sealed class ApplicationStoreUrisOptions
{
	/// <summary>
	/// The URL to the application store for Android.
	/// </summary>
	public Uri Android { get; set; }

	/// <summary>
	/// The URL to the application store for iOS.
	/// </summary>
	public Uri Ios { get; set; }
}
