using System;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Provides common review prompt conditions.
/// </summary>
public static class ReviewConditionCustom
{
	/// <summary>
	/// The application onboarding must be completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <returns>Review prompt condition which can be used to determine whether the user as completed the onboarding.</returns>
	public static ReviewConditionCallback<TReviewSettings> OnBoardingCompleted<TReviewSettings>()
		where TReviewSettings : ReviewSettingsCustom =>
			(ct, reviewSettings, currentDateTime) => Task.FromResult(reviewSettings.HasCompletedOnboarding is true);
}
