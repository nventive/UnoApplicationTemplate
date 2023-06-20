using System;
using System.Threading;
using System.Threading.Tasks;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// Extensions of <see cref="IReviewService{TReviewSettings}"/>.
/// </summary>
public static class ReviewServiceExtensions
{
	/// <summary>
	/// Tracks that the application was launched.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="reviewService"><see cref="IReviewService{TReviewSettings}"/>.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static async Task TrackApplicationLaunched<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettings
	{
		await reviewService.UpdateReviewSettings(ct, reviewSettings =>
		{
			return reviewSettings.ApplicationFirstLaunched is null
				? (reviewSettings with
				{
					ApplicationFirstLaunched = DateTimeOffset.Now,
					ApplicationLaunchCount = reviewSettings.ApplicationLaunchCount + 1
				})
				: (reviewSettings with
				{
					ApplicationLaunchCount = reviewSettings.ApplicationLaunchCount + 1,
				});
		});
	}

	/// <summary>
	/// Tracks that a primary action was completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="reviewService"><see cref="IReviewService{TReviewSettings}"/>.</param>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static async Task TrackPrimaryActionCompleted<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettings
	{
		await reviewService.UpdateReviewSettings(
			ct,
			reviewSettings => reviewSettings with { PrimaryActionCompletedCount = reviewSettings.PrimaryActionCompletedCount + 1 }
		);
	}

	/// <summary>
	/// Tracks that a secondary action was completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="reviewService"><see cref="IReviewService{TReviewSettings}"/>.</param>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static async Task TrackSecondaryActionCompleted<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettings
	{
		await reviewService.UpdateReviewSettings(
			ct,
			reviewSettings => reviewSettings with { SecondaryActionCompletedCount = reviewSettings.SecondaryActionCompletedCount + 1 }
		);
	}
}
