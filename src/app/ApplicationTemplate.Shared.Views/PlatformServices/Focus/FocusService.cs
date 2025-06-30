#if __ANDROID__ || __IOS__
using System;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;

namespace ApplicationTemplate.DataAccess.PlatformServices;

/// <inheritdoc/>
public sealed partial class FocusService : IFocusService
{
	private readonly ILogger<FocusService> _logger;
	private readonly DispatcherQueue _dispatcherQueue;

	public FocusService(ILogger<FocusService> logger, DispatcherQueue dispatcherQueue)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
	}
}
#endif
