using System;

namespace ApplicationTemplate.Business;

/// <summary>
/// Service that handles the kill switch.
/// </summary>
public interface IKillSwitchService
{
	/// <summary>
	/// Observes and reports the activation or deactivation of the kill switch.
	/// </summary>
	/// <returns>
	/// An <see cref="IObservable{Boolean}"/> that indicates whether the kill switch is activated or not.
	/// </returns>
	public IObservable<bool> ObserveKillSwitchActivation();
}
