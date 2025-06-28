using System.Diagnostics.CodeAnalysis;

namespace CPS.DataAccess.PlatformServices;

/// <summary>
/// Represents toast notification durations.
/// </summary>
[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "It's within an enum.")]
[SuppressMessage("Design", "CA1008:Enums should have zero value", Justification = "It doesn't make sense to have a zero duration.")]
public enum ToastDuration
{
	/// <summary>
	/// 2000 milliseconds.
	/// </summary>
	Short = 2000,

	/// <summary>
	/// 3500 milliseconds.
	/// </summary>
	Long = 3500,
}
