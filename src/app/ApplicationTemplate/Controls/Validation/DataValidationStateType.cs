namespace ApplicationTemplate;

public enum DataValidationStateType
{
	/// <summary>
	/// The field doesn't have any validation yet (not error, nor valid).
	/// </summary>
	Default = 0,

	/// <summary>
	/// Data is invalid.
	/// </summary>
	Error = 1,

	/// <summary>
	/// Data is valid.
	/// </summary>
	Valid = 2,
}
