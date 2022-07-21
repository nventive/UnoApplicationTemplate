using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate
{
	public static class DiagnosticsConfiguration
	{
		/// <summary>
		/// Adds the various diagnostics services and decorators.
		/// </summary>
		/// <param name="services">The service collection.</param>
		/// <returns>The provided <paramref name="services"/>.</returns>
		/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
		public static IServiceCollection AddDiagnostics(this IServiceCollection services, IConfiguration configuration)
		{
			return services.BindOptionsToConfiguration<DiagnosticsOptions>(configuration);
		}
	}

	public class DiagnosticsOptions
	{
		public DiagnosticsOptions()
		{
//-:cnd:noEmit
#if DEBUG
			IsDiagnosticsOverlayEnabled = true;
#endif
//+:cnd:noEmit
		}

		public bool IsDiagnosticsOverlayEnabled { get; set; }
	}
}
