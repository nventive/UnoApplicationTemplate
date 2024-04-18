using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ApplicationTemplate;

public class MockThreadCultureOverrideService : IThreadCultureOverrideService
{
	public CultureInfo GetCulture()
	{
		return CultureInfo.CurrentCulture;
	}

	public void SetCulture(CultureInfo culture)
	{
		return;
	}

	public bool TryApply()
	{
		return false;
	}
}
