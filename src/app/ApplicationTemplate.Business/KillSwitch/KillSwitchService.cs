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
	private readonly IKillSwitchRepository _killSwitchRepository;
	private readonly ILogger<KillSwitchService> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="KillSwitchService"/> class.
	/// </summary>
	/// <param name="killSwitchRepository">The <see cref="IKillSwitchRepository"/>.</param>
	/// <param name="logger">The <see cref="ILogger{KillSwitchService}"/>.</param>
	public KillSwitchService(IKillSwitchRepository killSwitchRepository, ILogger<KillSwitchService> logger)
	{
		_killSwitchRepository = killSwitchRepository;
		_logger = logger;
	}

	/// <inheritdoc/>
	public IObservable<bool> ObserveKillSwitchActivation() => _killSwitchRepository.ObserveKillSwitchActivation()
		.Do(isActive => _logger.LogInformation("Kill switch is now {IsActive}.", isActive));
}
