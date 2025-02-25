using System;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides memory informations used for diagnostics.
/// </summary>
/// <remarks>
/// Please note that <see cref="ObservePrivateMemorySize"/> doesn't work on iOS for now.
/// See https://github.com/dotnet/runtime/issues/86251 for more details.
/// </remarks>
public interface IMemoryProvider
{
	/// <summary>
	/// Observes the amount of private memory, in bytes, allocated for the application.
	/// </summary>
	/// <remarks>
	/// See https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.process.privatememorysize64?view=net-6.0 for more information about the API used.
	/// </remarks>
	/// <returns>Amount of bytes allocated for the application.</returns>
	IObservable<long> ObservePrivateMemorySize();

	/// <summary>
	/// Observes the approximation of the number of bytes currently allocated in managed memory.
	/// </summary>
	/// <remarks>
	/// See https://learn.microsoft.com/en-us/dotnet/api/system.gc.gettotalmemory?view=net-6.0 for more information about the API used.
	/// See https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals#the-managed-heap for more information about managed memory.
	/// </remarks>
	/// <returns>The heap size in bytes.</returns>
	IObservable<long> ObserveManagedMemorySize();
}
