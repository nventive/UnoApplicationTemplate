using System;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Provides common review prompt conditions.
/// </summary>
public static class ReviewCondition
{
	/// <summary>
	/// The number of completed primary actions must be at least <paramref name="minimumActionCompleted"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="minimumActionCompleted">The minimum number of completed actions.</param>
	/// <returns>Review prompt condition which can be used to determine whether the user as completed enough primary actions.</returns>
	public static ReviewConditionCallback<TReviewSettings> PrimaryActionCompletedAtLeast<TReviewSettings>(int minimumActionCompleted)
		where TReviewSettings : ReviewSettings =>
			(ct, reviewSettings, currentDateTime) => Task.FromResult(reviewSettings.PrimaryActionCompletedCount >= minimumActionCompleted);

	/// <summary>
	/// The number of completed secondary actions must be at least <paramref name="minimumActionCompleted"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="minimumActionCompleted">The minimum number of completed actions.</param>
	/// <returns>Review prompt condition which can be used to determine whether the user as completed enough primary actions.</returns>
	public static ReviewConditionCallback<TReviewSettings> SecondaryActionCompletedAtLeast<TReviewSettings>(int minimumActionCompleted)
		where TReviewSettings : ReviewSettings =>
			(ct, reviewSettings, currentDateTime) => Task.FromResult(reviewSettings.SecondaryActionCompletedCount >= minimumActionCompleted);

	/// <summary>
	/// The number of times the application has been launched must be at least <paramref name="minimumApplicationLaunched"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="minimumApplicationLaunched">The minimum number of times the application has been launched.</param>
	/// <returns>Review prompt condition which can be used to determine whether the user as launched the application enough times.</returns>
	public static ReviewConditionCallback<TReviewSettings> ApplicationLaunchedAtLeast<TReviewSettings>(int minimumApplicationLaunched)
		where TReviewSettings : ReviewSettings =>
			(ct, reviewSettings, currentDateTime) => Task.FromResult(reviewSettings.ApplicationLaunchCount >= minimumApplicationLaunched);

	/// <summary>
	/// The time elapsed since the first application launch must be at least <paramref name="minimumTimeElapsed"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="minimumTimeElapsed">The minimum time elapsed since the the first application launch.</param>
	/// <returns>Review prompt condition which can be used to determine whether the elapsed time since the first application launch is enough.</returns>
	public static ReviewConditionCallback<TReviewSettings> ApplicationFirstLaunchedAtLeast<TReviewSettings>(TimeSpan minimumTimeElapsed)
		where TReviewSettings : ReviewSettings =>
			(ct, reviewSettings, currentDateTime) =>
				Task.FromResult(reviewSettings.ApplicationFirstLaunched.HasValue && reviewSettings.ApplicationFirstLaunched.Value + minimumTimeElapsed <= currentDateTime);
}
