using System;
using System.Threading;
using System.Threading.Tasks;
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

	public ReviewService(
		IReviewPrompter reviewPrompter,
		IReviewSettingsSource<TReviewSettings> reviewSettingsSource,
		ReviewConditionCallback<TReviewSettings>[] reviewConditions
	)
	{
		_reviewPrompter = reviewPrompter;
		_reviewSettingsSource = reviewSettingsSource;
		_reviewConditions = reviewConditions;
	}

	/// <inheritdoc/>
	public Task TryRequestReview(CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public Task<bool> GetAreConditionsSatisfied(CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public Task UpdateReviewSettings(CancellationToken ct, Func<TReviewSettings, TReviewSettings> updateFunction)
	{
		throw new NotImplementedException();
	}
}
