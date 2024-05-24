#if __ANDROID__
using System;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess;
using Firebase.RemoteConfig;

namespace ApplicationTemplate.Views;

/// <summary>
/// RemoteConfigRepository is a repository for Firebase Remote Config.
/// </summary>
public sealed class RemoteConfigRepository : IKillSwitchRepository, IMinimumVersionReposiory, IDisposable
{
	private Subject<bool> _killSwitchSubject = new Subject<bool>();
	private Subject<Version> _versionSubject = new Subject<Version>();

	/// <summary>
	/// Initializes a new instance of the <see cref="RemoteConfigRepository"/> class.
	/// </summary>
	public RemoteConfigRepository()
	{
		FirebaseRemoteConfig mFirebaseRemoteConfig = FirebaseRemoteConfig.Instance;
		FirebaseRemoteConfigSettings configSettings = new FirebaseRemoteConfigSettings.Builder()
				.SetMinimumFetchIntervalInSeconds(3600)
				.Build();
		mFirebaseRemoteConfig.SetConfigSettingsAsync(configSettings);

		FetchRemoteConfig();
		ListenForRealTimeChanges();
	}

	private void FetchRemoteConfig()
	{
		FirebaseRemoteConfig mFirebaseRemoteConfig = FirebaseRemoteConfig.Instance;
		mFirebaseRemoteConfig.FetchAndActivate()
			.AddOnCompleteListener(new FetchCompleteListener(this));
	}

	private void ListenForRealTimeChanges()
	{
		FirebaseRemoteConfig mFirebaseRemoteConfig = FirebaseRemoteConfig.Instance;

		mFirebaseRemoteConfig.AddOnConfigUpdateListener(new ConfigUpdateListener(this));
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

	private sealed class FetchCompleteListener : Java.Lang.Object, Android.Gms.Tasks.IOnCompleteListener
	{
		private readonly RemoteConfigRepository _repository;

		public FetchCompleteListener(RemoteConfigRepository repository)
		{
			_repository = repository;
		}

		public void OnComplete(Android.Gms.Tasks.Task task)
		{
			if (task.IsSuccessful)
			{
				_repository._killSwitchSubject.OnNext(FirebaseRemoteConfig.Instance.GetBoolean("kill_switch"));
				_repository._versionSubject.OnNext(new Version(FirebaseRemoteConfig.Instance.GetString("minimum_version")));
			}
		}
	}

	private sealed class ConfigUpdateListener : Java.Lang.Object, IConfigUpdateListener
	{
		private readonly RemoteConfigRepository _repository;

		public ConfigUpdateListener(RemoteConfigRepository repository)
		{
			_repository = repository;
		}

		public void OnError(FirebaseRemoteConfigException p0)
		{
			throw new NotImplementedException();
		}

		public void OnUpdate(ConfigUpdate p0)
		{
			var instance = FirebaseRemoteConfig.Instance;

			instance.FetchAndActivate();
			var isKillSwitchActivated = FirebaseRemoteConfig.Instance.GetBoolean("kill_switch");
			var minimumVersion = new Version(FirebaseRemoteConfig.Instance.GetString("minimum_version"));

			_repository._killSwitchSubject.OnNext(isKillSwitchActivated);
			_repository._versionSubject.OnNext(minimumVersion);
		}
	}
}
#endif
