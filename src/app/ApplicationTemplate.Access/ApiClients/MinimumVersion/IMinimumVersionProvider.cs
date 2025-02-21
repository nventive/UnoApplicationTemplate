using System;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Gets the minimum version required to use the application.
/// </summary>
public interface IMinimumVersionProvider
{
	/// <summary>
	/// Checks the minimum required version.
	/// </summary>
	void CheckMinimumVersion();

	/// <summary>
	/// An observable that emits the minimum version required to use the application.
	/// </summary>
	IObservable<Version> MinimumVersionObservable { get; }
}
