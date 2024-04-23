using System;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Interface for the repository that gets when the kill switch is activated.
/// </summary>
public interface IKillSwitchRepository
{
	/// <summary>
	/// Observes and reports the activation or deactivation of the kill switch.
	/// </summary>
	/// <returns>
	/// An <see cref="IObservable{Boolean}"/> that indicates whether the kill switch is activated or not.
	/// </returns>
	public IObservable<bool> ObserveKillSwitchActivation();
}
