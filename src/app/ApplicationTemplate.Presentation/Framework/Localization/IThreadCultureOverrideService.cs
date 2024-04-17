using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ApplicationTemplate;

/// <summary>
/// Allows to override the culture of the threads.
/// This is used to change the language of the application.
/// </summary>
public interface IThreadCultureOverrideService
{
	/// <summary>
	/// Gets the culture override set using the <see cref="SetCulture"/> method.
	/// </summary>
	/// <returns>Current culture override. Null if not set.</returns>
	CultureInfo GetCulture();

	/// <summary>
	/// Sets the specified <paramref name="culture"/> as the culture override.
	/// To apply these changes, use the <see cref="TryApply"/> method.
	/// </summary>
	/// <param name="culture">The culture to use.</param>
	void SetCulture(CultureInfo culture);

	/// <summary>
	/// If there was a culture override set using the <see cref="SetCulture"/> method,
	/// then this method will apply the culture override on top of the system culture.
	/// </summary>
	/// <returns>True if the culture was overwritten; false otherwise.</returns>
	bool TryApply();
}
