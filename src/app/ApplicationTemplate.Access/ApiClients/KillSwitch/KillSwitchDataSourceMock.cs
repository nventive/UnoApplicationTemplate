using System;
using System.Reactive.Subjects;
using DynamicData;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Mock implementation of the kill switch repository.
/// </summary>
public sealed class KillSwitchDataSourceMock : IKillSwitchDataSource, IDisposable
{
	private readonly Subject<bool> _killSwitchActivatedSubject = new();
	private bool _killSwitchActivated = false;

	/// <inheritdoc/>
	public IObservable<bool> ObserveKillSwitchActivation() => _killSwitchActivatedSubject;

	/// <summary>
	/// Change the kill switch activation status.
	/// </summary>
	public void ChangeKillSwitchActivation()
	{
		_killSwitchActivated = !_killSwitchActivated;

		_killSwitchActivatedSubject.OnNext(_killSwitchActivated);
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		_killSwitchActivatedSubject.Dispose();
	}
}
