// src/app/ApplicationTemplate.Shared.Views/PlatformServices/PhoneCall/PhoneCallService.cs
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls;
using Windows.System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class PhoneCallService : IPhoneCallService
{
	private readonly Subject<Unit> _callStateSubject = new();

	public PhoneCallService()
	{
#if __ANDROID__ || __IOS__ || WINDOWS
		// Subscribe to call state changes if available on the platform
		try
		{
			PhoneCallManager.CallStateChanged += OnCallStateChanged;
		}
		catch
		{
			// Call state monitoring might not be available on all platforms
		}
#endif
	}

	public async Task<bool> GetIsCallActive()
	{
#if __ANDROID__ || __IOS__ || WINDOWS
		try
		{
			return PhoneCallManager.IsCallActive;
		}
		catch
		{
			return false;
		}
#else
		return await Task.FromResult(false);
#endif
	}

	public async Task<bool> GetIsCallIncoming()
	{
#if __ANDROID__ || __IOS__ || WINDOWS
		try
		{
			return PhoneCallManager.IsCallIncoming;
		}
		catch
		{
			return false;
		}
#else
		return await Task.FromResult(false);
#endif
	}

	public void OpenPhoneCall(string phoneNumber)
	{
		if (string.IsNullOrWhiteSpace(phoneNumber))
			return;

#if __ANDROID__ || __IOS__ || WINDOWS
		try
		{
			PhoneCallManager.ShowPhoneCallUI(phoneNumber, string.Empty);
		}
		catch
		{
			// Fallback to launcher if PhoneCallManager is not available
			_ = Launcher.LaunchUriAsync(new Uri($"tel:{phoneNumber}"));
		}
#else
		// For other platforms, use the launcher
		_ = Launcher.LaunchUriAsync(new Uri($"tel:{phoneNumber}"));
#endif
	}

	public void OpenPhoneCallSettings()
	{
#if __ANDROID__ || WINDOWS
		try
		{
			PhoneCallManager.ShowPhoneCallSettingsUI();
		}
		catch
		{
			// Settings might not be available, silently fail
		}
#endif
		// Not supported on iOS as mentioned in the interface
	}

	public IObservable<Unit> ObserveCallState()
	{
		return _callStateSubject.AsObservable();
	}

#if __ANDROID__ || __IOS__ || WINDOWS
	private void OnCallStateChanged(object sender, object e)
	{
		_callStateSubject.OnNext(Unit.Default);
	}
#endif

	public void Dispose()
	{
#if __ANDROID__ || __IOS__ || WINDOWS
		try
		{
			PhoneCallManager.CallStateChanged -= OnCallStateChanged;
		}
		catch
		{
			// Ignore disposal errors
		}
#endif
		_callStateSubject?.Dispose();
	}
}
