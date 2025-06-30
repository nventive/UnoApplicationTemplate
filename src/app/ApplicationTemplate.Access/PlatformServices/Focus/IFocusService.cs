namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <summary>
/// Provides access to native methods to manage focus within the application.
/// </summary>
public interface IFocusService
{
	/// <summary>
	/// Clears the focus from the currently focused element.
	/// </summary>
	/// <remarks>
	/// This method is not supported on Windows.
	/// </remarks>
	void ClearFocus();
}
