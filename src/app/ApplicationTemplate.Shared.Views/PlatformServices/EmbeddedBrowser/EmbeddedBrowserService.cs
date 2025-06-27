// src/app/ApplicationTemplate.Shared.Views/PlatformServices/EmbeddedBrowser/EmbeddedBrowserService.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;  // Assuming namespace from IEmbeddedBrowserService

namespace ApplicationTemplate.Views.PlatformServices.EmbeddedBrowser
{
	public partial class EmbeddedBrowserService : IEmbeddedBrowserService
	{
		public virtual Task NavigateTo(CancellationToken ct, Uri uri)
		{
			// This method should be overridden in platform-specific implementations.
			// Throwing here ensures it's not called on unsupported platforms.
			throw new NotImplementedException("Embedded browser is not supported on this platform.");
		}
	}
}
