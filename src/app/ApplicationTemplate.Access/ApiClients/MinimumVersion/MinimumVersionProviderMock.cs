using System;
using System.Reactive.Subjects;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// A mock implementation of the minimum version provider. Used for testing.
/// </summary>
public sealed class MinimumVersionProviderMock : IMinimumVersionProvider, IDisposable
{
	private readonly Subject<Version> _minimumVersionSubject = new();

	/// <inheritdoc/>
	public IObservable<Version> MinimumVersionObservable => _minimumVersionSubject;

	/// <inheritdoc/>
	public void CheckMinimumVersion()
	{
		_minimumVersionSubject.OnNext(new Version(1, 0, 0, 0));
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_minimumVersionSubject.Dispose();
	}
}
