#if __IOS__
using System;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess;
using Firebase.RemoteConfig;
using Foundation;

namespace ApplicationTemplate.Views;

/// <summary>
/// Implementation of the killswitch repository and the minimum version with Firebase Remote Config for iOS.
/// </summary>
public sealed class RemoteConfigRepository : IKillSwitchRepository, IMinimumVersionReposiory, IDisposable
{
	private BehaviorSubject<bool> _killSwitchSubject = new BehaviorSubject<bool>(default);
	private BehaviorSubject<Version> _versionSubject = new BehaviorSubject<Version>(default);

	/// <summary>
	///  Initializes a new instance of the <see cref="RemoteConfigRepository"/> class.
	/// </summary>
	public RemoteConfigRepository()
	{
		Firebase.Core.App.Configure();
		// Enabling developer mode, allows for frequent refreshes of the cache
		RemoteConfig.SharedInstance.ConfigSettings = new RemoteConfigSettings();

		object[] values = { false, "1.0.0" };
		object[] keys = { "kill_switch", "minimum_version" };
		var defaultValues = NSDictionary.FromObjectsAndKeys(values, keys, keys.Length);
		RemoteConfig.SharedInstance.SetDefaults(defaultValues);

		// CacheExpirationSeconds is set to CacheExpiration here, indicating that any previously
		// fetched and cached config would be considered expired because it would have been fetched
		// more than CacheExpiration seconds ago. Thus the next fetch would go to the server unless
		// throttling is in progress. The default expiration duration is 43200 (12 hours).
		RemoteConfig.SharedInstance.Fetch(10, (status, error) =>
		{
			switch (status)
			{
				case RemoteConfigFetchStatus.Success:
					Console.WriteLine("Config Fetched!");

					// Call this method to make fetched parameter values available to your app
					RemoteConfig.SharedInstance.Activate();

					_killSwitchSubject.OnNext(RemoteConfig.SharedInstance["kill_switch"].BoolValue);
					_versionSubject.OnNext(new Version(RemoteConfig.SharedInstance["minimum_version"].StringValue));
					break;

				case RemoteConfigFetchStatus.Throttled:
				case RemoteConfigFetchStatus.NoFetchYet:
				case RemoteConfigFetchStatus.Failure:
					Console.WriteLine("Config not fetched...");
					break;
			}
		});

		RemoteConfig.SharedInstance.AddObserver(new NSString("kill_switch"), NSKeyValueObservingOptions.New, (nSObservedChange) =>
		{
			if (nSObservedChange.NewValue is NSNumber nSNumber)
			{
				_killSwitchSubject.OnNext(nSNumber.BoolValue);
			}
		});
		RemoteConfig.SharedInstance.AddObserver(new NSString("minimum_version"), NSKeyValueObservingOptions.New, (nSObservedChange) =>
		{
			if (nSObservedChange.NewValue is NSString nSString)
			{
				_versionSubject.OnNext(new Version(nSString));
			}
		});
	}

	/// <inheritdoc />
	public IObservable<Version> MinimumVersionObservable => _versionSubject;

	/// <inheritdoc />
	public void CheckMinimumVersion()
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public IObservable<bool> ObserveKillSwitchActivation()
	{
		return _killSwitchSubject;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_killSwitchSubject.Dispose();
		_versionSubject.Dispose();
	}
}
#endif
