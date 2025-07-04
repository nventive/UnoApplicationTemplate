using System;
using System.Reactive.Subjects;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class FakeOrientationProvider : IOrientationProvider, IDisposable
{
	private readonly BehaviorSubject<bool> _isLandscapeSubject = new BehaviorSubject<bool>(false);

	public IObservable<bool> GetAndObserveIsLandscape()
	{
		return _isLandscapeSubject;
	}

	public bool GetIsLandscape()
	{
		return _isLandscapeSubject.Value;
	}

	public void SetIsLandscape(bool isLandscape)
	{
		_isLandscapeSubject.OnNext(isLandscape);
	}

	public void Dispose()
	{
		_isLandscapeSubject.Dispose();
	}
}
