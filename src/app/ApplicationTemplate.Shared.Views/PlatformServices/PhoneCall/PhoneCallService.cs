using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Windows.ApplicationModel.Calls;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// The <see cref="IPhoneCallService"/> implementation using Uno.
/// </summary>
public sealed class PhoneCallService : IPhoneCallService
{
	private readonly DispatcherQueue _dispatcherQueue;

	public PhoneCallService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
	}

	/// <inheritdoc/>
	public async Task<bool> GetIsCallActive()
	{
		return await _dispatcherQueue.EnqueueAsync(() => PhoneCallManager.IsCallActive);
	}

	/// <inheritdoc/>
	public async Task<bool> GetIsCallIncoming()
	{
		return await _dispatcherQueue.EnqueueAsync(() => PhoneCallManager.IsCallIncoming);
	}

	/// <inheritdoc/>
	public void OpenPhoneCall(string phoneNumber)
	{
		_dispatcherQueue.TryEnqueue(() => PhoneCallManager.ShowPhoneCallUI(phoneNumber, string.Empty));
	}

	/// <inheritdoc/>
	public void OpenPhoneCallSettings()
	{
#if __IOS__
		return;
#else
		_dispatcherQueue.TryEnqueue(PhoneCallManager.ShowPhoneCallSettingsUI);
#endif
	}

	/// <inheritdoc/>
	public IObservable<Unit> ObserveCallState()
	{
		return Observable.FromEventPattern<EventHandler<object>, object>(
			h => PhoneCallManager.CallStateChanged += h,
			h => PhoneCallManager.CallStateChanged -= h
		)
		.Select(_ => Unit.Default);
	}
}
