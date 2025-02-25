using System;
using System.Reactive.Linq;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.Logging;

namespace ApplicationTemplate.Business;

/// <summary>
/// Implementation of the IKillSwitchService.
/// </summary>
public sealed class KillSwitchService : IKillSwitchService
{
	private readonly IKillSwitchDataSource _killSwitchDataSource;
	private readonly ILogger<KillSwitchService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="KillSwitchService"/> class.
	/// </summary>
	/// <param name="killSwitchDataSource">The <see cref="IKillSwitchDataSource"/>.</param>
	/// <param name="logger">The <see cref="ILogger{KillSwitchService}"/>.</param>
	public KillSwitchService(IKillSwitchDataSource killSwitchDataSource, ILogger<KillSwitchService> logger)
	{
		_killSwitchDataSource = killSwitchDataSource;
		_logger = logger;
	}

	/// <inheritdoc/>
	public IObservable<bool> ObserveKillSwitchActivation() => _killSwitchDataSource.ObserveKillSwitchActivation()
		.Do(isActive => _logger.LogInformation("Kill switch is now {IsActive}.", isActive));
}
