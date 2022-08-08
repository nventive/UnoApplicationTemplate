using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate;

/// <summary>
/// This implementation simply returns the provided resource name as if it were the value.
/// </summary>
public class MockStringLocalizer : IStringLocalizer
{
	public LocalizedString this[string name] => new LocalizedString(name, name);

	public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, name);

	public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
	{
		yield break;
	}

	public IStringLocalizer WithCulture(CultureInfo culture)
	{
		return this;
	}
}
