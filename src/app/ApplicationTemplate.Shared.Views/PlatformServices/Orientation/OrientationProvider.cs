// src/app/ApplicationTemplate.Shared.Views/PlatformServices/Orientation/OrientationProvider.cs
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess.PlatformServices;
using Microsoft.UI.Xaml;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class OrientationProvider : IOrientationProvider, IDisposable
{
	private readonly BehaviorSubject<bool> _isLandscapeSubject;
	private bool _disposed;

	public OrientationProvider()
	{
		var currentIsLandscape = GetCurrentIsLandscape();
		_isLandscapeSubject = new BehaviorSubject<bool>(currentIsLandscape);

		if (Window.Current != null)
		{
			Window.Current.SizeChanged += OnWindowSizeChanged;
		}
	}

	public bool GetIsLandscape()
	{
		return GetCurrentIsLandscape();
	}

	public IObservable<bool> GetAndObserveIsLandscape()
	{
		return _isLandscapeSubject.AsObservable();
	}

	private bool GetCurrentIsLandscape()
	{
		if (Window.Current?.Bounds != null)
		{
			var bounds = Window.Current.Bounds;
			return bounds.Width > bounds.Height;
		}
		return false;
	}

	private void OnWindowSizeChanged(object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs e)
	{
		if (_disposed) return;

		var isLandscape = e.Size.Width > e.Size.Height;
		_isLandscapeSubject.OnNext(isLandscape);
	}

	public void Dispose()
	{
		if (_disposed) return;

		if (Window.Current != null)
		{
			Window.Current.SizeChanged -= OnWindowSizeChanged;
		}

		_isLandscapeSubject?.Dispose();
		_disposed = true;
	}
}
