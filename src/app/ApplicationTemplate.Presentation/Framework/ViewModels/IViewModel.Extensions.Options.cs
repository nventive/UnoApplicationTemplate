using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace Chinook.DynamicMvvm
{
	public static class ChinookViewModelExtensionsForOptions
	{
		public static TOptions GetOptionsValue<TOptions>(this IViewModel viewModel)
			where TOptions : class, new()
		{
			return viewModel.GetService<IOptionsMonitor<TOptions>>().CurrentValue;
		}

		public static IOptionsMonitor<TOptions> GetOptionsMonitor<TOptions>(this IViewModel viewModel)
			where TOptions : class, new()
		{
			return viewModel.GetService<IOptionsMonitor<TOptions>>();
		}
	}
}
