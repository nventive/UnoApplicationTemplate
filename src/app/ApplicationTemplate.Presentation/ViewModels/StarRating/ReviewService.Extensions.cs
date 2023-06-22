using System.Threading;
using System.Threading.Tasks;
using ReviewService.Abstractions;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// Extensions of <see cref="IReviewService{TReviewSettings}"/>.
/// </summary>
public static class ReviewServiceExtensions
{
	/// <summary>
	/// Tracks that the application onboarding was completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="reviewService"><see cref="IReviewService{TReviewSettings}"/>.</param>
	/// <param name="ct">The cancellation token.</param>
	/// <returns><see cref="Task"/>.</returns>
	public static async Task TrackOnboardingCompleted<TReviewSettings>(this IReviewService<TReviewSettings> reviewService, CancellationToken ct)
		where TReviewSettings : ReviewSettingsCustom
	{
		await reviewService.UpdateReviewSettings(ct, reviewSettings =>
		{
			return reviewSettings with { HasCompletedOnboarding = reviewSettings.HasCompletedOnboarding };
		});
	}
}
