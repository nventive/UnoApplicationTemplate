namespace ReviewService.Abstractions;

/// <summary>
/// Provides common review prompt conditions.
/// </summary>
public static partial class ReviewConditionsBuilderExtensions
{
	/// <summary>
	/// The application onboarding must be completed.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder"><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> ApplicationOnboardingCompleted<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder)
		where TReviewSettings : ReviewSettingsCustom
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.HasCompletedOnboarding is true)
		);
		return builder;
	}
}
