// src/app/ApplicationTemplate.Shared.Views/PlatformServices/ScreenWakeLock/ScreenWakeLockService.cs
namespace ApplicationTemplate.DataAccess.PlatformServices
{
	using Android.OS;
	using UIKit;
	using Windows.System;

	public sealed class ScreenWakeLockService : IScreenWakeLockService
	{
		private object _lock;

		public void Enable()
		{
#if __ANDROID__
			if (_lock == null)
			{
				var pm = (PowerManager)Android.App.Application.Context.GetSystemService(Context.PowerService);
				_lock = pm.NewWakeLock(WakeLockFlags.ScreenBright | WakeLockFlags.OnAfterRelease, "MyApp:WakeLock");
				((PowerManager.WakeLock)_lock).Acquire();
			}
#elif __IOS__
			if (_lock == null)
			{
				_lock = new NSObject();
				UIApplication.SharedApplication.IdleTimerDisabled = true;
			}
#elif __WINDOWS__
			if (_lock == null)
			{
				_lock = new DisplayRequest();
				((DisplayRequest)_lock).RequestActive();
			}
#endif
		}

		public void Disable()
		{
#if __ANDROID__
			if (_lock != null)
			{
				((PowerManager.WakeLock)_lock).Release();
				_lock = null;
			}
#elif __IOS__
			if (_lock != null)
			{
				UIApplication.SharedApplication.IdleTimerDisabled = false;
				((NSObject)_lock).Dispose();
				_lock = null;
			}
#elif __WINDOWS__
			if (_lock != null)
			{
				((DisplayRequest)_lock).RequestRelease();
				_lock = null;
			}
#endif
		}
	}
}
