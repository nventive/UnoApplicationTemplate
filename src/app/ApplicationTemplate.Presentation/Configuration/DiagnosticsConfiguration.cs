using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTemplate;

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
		return services
			.BindOptionsToConfiguration<DiagnosticsOptions>(configuration)
			.AddSingleton<IHttpDebuggerService, HttpDebuggerService>();
	}
}

public class DiagnosticsOptions
{
	public DiagnosticsOptions()
	{
//-:cnd:noEmit
#if DEBUG
		IsDiagnosticsOverlayEnabled = true;
		IsHttpDebuggerEnabled = true;
#endif
//+:cnd:noEmit
	}

	public bool IsDiagnosticsOverlayEnabled { get; set; }

	public bool IsDiagnosticsOverlayOnTheLeft { get; set; }

	public bool IsHttpDebuggerEnabled { get; set; }

	public HttpDebuggerOptions HttpDebugger { get; set; } = new();
}

public class HttpDebuggerOptions
{
	public bool HideRequestHeaders { get; set; }

	public bool HideResponseHeaders { get; set; }

	public bool FormatRequestContent { get; set; }

	public bool FormatResponseContent { get; set; }
}
