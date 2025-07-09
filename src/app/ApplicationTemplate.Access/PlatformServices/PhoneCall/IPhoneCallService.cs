using System;
using System.Reactive;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides access to native methods for phone call operations on the device.
/// </summary>
public interface IPhoneCallService
{
	/// <summary>
	/// Gets a value that indicates whether an active call is in progress on the device.
	/// </summary>
	bool IsCallActive { get; }

	/// <summary>
	/// Gets a value that indicates if a call is incoming on the device.
	/// </summary>
	bool IsCallIncoming { get; }

	/// <summary>
	/// Opens the built-in phone call UI with the specified phone number.
	/// </summary>
	/// <param name="phoneNumber">The phone number.</param>
	void OpenPhoneCall(string phoneNumber);

	/// <summary>
	/// Opens the call settings UI.
	/// </summary>
	/// <remarks>
	/// Not supported on iOS.
	/// </remarks>
	void OpenPhoneCallSettings();

	/// <summary>
	/// Observes the call state changes on the device.
	/// Occurs when a call is incoming, accepted or dropped.
	/// </summary>
	/// <returns>An observable sequence yielding a <see cref="Unit"/> value when the call states changes.</returns>
	IObservable<Unit> ObserveCallState();
}
