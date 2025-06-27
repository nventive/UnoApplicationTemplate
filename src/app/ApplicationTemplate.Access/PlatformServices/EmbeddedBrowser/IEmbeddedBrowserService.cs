using System;
using System.Threading;
using System.Threading.Tasks;

namespace CPS.DataAccess;

/// <summary>
/// Defines a contract dedicated to launching the embedded browser.
/// </summary>
public interface IEmbeddedBrowserService
{
	/// <summary>
	/// This function call the browser with an uri.
	/// </summary>
	/// <param name="ct">Cancellation Token.</param>
	/// <param name="uri">The <see cref="Uri"/>.</param>
	Task NavigateTo(CancellationToken ct, Uri uri);
}
