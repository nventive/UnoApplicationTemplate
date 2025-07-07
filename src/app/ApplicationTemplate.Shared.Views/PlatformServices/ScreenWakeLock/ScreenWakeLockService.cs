using System;
using Microsoft.UI.Dispatching;
using Windows.System.Display;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Implementation of <see cref="IScreenWakeLockService"/> using Windows Display Request API.
/// </summary>
public sealed class ScreenWakeLockService : IScreenWakeLockService, IDisposable
{
	private readonly DispatcherQueue _dispatcherQueue;

	private DisplayRequest _displayRequest;
	private bool _isEnabled;

	public ScreenWakeLockService(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
	}

	/// <inheritdoc/>
	public void Enable()
	{
		if (_isEnabled)
		{
			return;
		}

		_dispatcherQueue.TryEnqueue(() =>
		{
			_displayRequest ??= new DisplayRequest();
			_displayRequest.RequestActive();
		});

		_isEnabled = true;
	}

	/// <inheritdoc/>
	public void Disable()
	{
		if (!_isEnabled)
		{
			throw new Exception("Screen wake lock is not currently enabled.");
		}

		_displayRequest.RequestRelease();
		_isEnabled = false;
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		if (_isEnabled)
		{
			Disable();
		}
	}
}
