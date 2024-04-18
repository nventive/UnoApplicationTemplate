using System;

namespace ApplicationTemplate.Business;

/// <summary>
/// This service checks if the application is running the minimum required version.
/// </summary>
public interface IUpdateRequiredService : IDisposable
{
	/// <summary>
	/// Event that is raised when the application needs to be updated.
	/// </summary>
	/// <remarks>This is using a plain event with no arguments because this is an event that should only be raised once,
	/// and requires the application to be relaunched after being updated.</remarks>
	event EventHandler UpdateRequired;
}
