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
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	public static Task TrackApplicationLaunched<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettings
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Tracks that a primary action was completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="reviewService"><see cref="IReviewService{TReviewSettings}"/>.</param>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static Task TrackPrimaryActionCompleted<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettings
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Tracks that a secondary action was completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="reviewService"><see cref="IReviewService{TReviewSettings}"/>.</param>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static Task TrackSecondaryActionCompleted<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettings
	{
		throw new NotImplementedException();
	}
}
