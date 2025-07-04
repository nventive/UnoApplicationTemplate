using System;
using System.Reactive.Subjects;
using Windows.UI.ViewManagement;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public class OrientationProvider : IOrientationProvider, IDisposable
{
	private readonly BehaviorSubject _isLandscapeSubject;
	private readonly ApplicationView _applicationView;

	public OrientationProvider()
	{
		_applicationView = ApplicationView.GetForCurrentView();
		_isLandscapeSubject = new BehaviorSubject(GetIsLandscape());
		_applicationView.OrientationChanged += OnOrientationChanged;
	}

	private void OnOrientationChanged(ApplicationView sender, object args)
	{
		_isLandscapeSubject.OnNext(GetIsLandscape());
	}

	public bool GetIsLandscape()
	{
		return _applicationView.Orientation == ApplicationViewOrientation.Landscape;
	}

	public IObservable GetAndObserveIsLandscape()
	{
		return _isLandscapeSubject.AsObservable();
	}

	public void Dispose()
	{
		_applicationView.OrientationChanged -= OnOrientationChanged;
		_isLandscapeSubject.Dispose();
	}
}
