using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// Implementation of <see cref="IReviewService{TReviewSettings}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public sealed class ReviewService<TReviewSettings> : IReviewService<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	private readonly IReviewPrompter _reviewPrompter;
	private readonly IReviewSettingsSource<TReviewSettings> _reviewSettingsSource;
	private readonly ReviewConditionCallback<TReviewSettings>[] _reviewConditions;
	private readonly ILogger _logger;

	public ReviewService(
		ILogger<ReviewService<TReviewSettings>> logger,
		IReviewPrompter reviewPrompter,
		IReviewSettingsSource<TReviewSettings> reviewSettingsSource,
		ReviewConditionCallback<TReviewSettings>[] reviewConditions
	)
	{
		_logger = logger;
		_reviewPrompter = reviewPrompter;
		_reviewSettingsSource = reviewSettingsSource;
		_reviewConditions = reviewConditions;
	}

	/// <inheritdoc/>
	public async Task TryRequestReview(CancellationToken ct)
	{
		_logger.LogDebug("Trying to request a review.");

		if (await GetAreConditionsSatisfied(ct))
		{
			await _reviewPrompter.TryPrompt();
			_logger.LogInformation("Review requested.");
		}
		else
		{
			_logger.LogInformation("Failed to request a review because one or more conditions were not satisfied.");
		}
	}

	/// <inheritdoc/>
	public async Task<bool> GetAreConditionsSatisfied(CancellationToken ct)
	{
		_logger.LogDebug("Evaluating conditions.");

		var currentSettings = await _reviewSettingsSource.Read(ct);
		var reviewConditionTasks = _reviewConditions.Select(async condition => await condition(ct, currentSettings, DateTimeOffset.Now));

		var result = (await Task.WhenAll(reviewConditionTasks)).All(x => x is true);

		if (result)
		{
			_logger.LogInformation("Evaluated conditions and all conditions are satisfied.");
		}
		else
		{
			_logger.LogInformation("Evaluted conditions and one or more conditions were not satisfied.");
		}

		return result;
	}

	/// <inheritdoc/>
	public async Task UpdateReviewSettings(CancellationToken ct, Func<TReviewSettings, TReviewSettings> updateFunction)
	{
		_logger.LogDebug("Updating review settings.");

		var currentSettings = await _reviewSettingsSource.Read(ct);

		try
		{
			await _reviewSettingsSource.Write(ct, updateFunction(currentSettings));

			_logger.LogInformation("Updated review settings.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to update review settings.");
		}
	}
}
