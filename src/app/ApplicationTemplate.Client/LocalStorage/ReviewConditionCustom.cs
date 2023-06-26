using System;

namespace ReviewService.Abstractions;

public static partial class ReviewConditionBuilderExtensions
{
	public static IReviewConditionsBuilder<TReviewSettings> OnBoardingCompleted<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder)
		where TReviewSettings : ReviewSettingsCustom
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.HasCompletedOnboarding is true)
		);
		return builder;
	}
}
