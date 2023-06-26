using System;

namespace ReviewService.Abstractions;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1625:Element documentation should not be copied and pasted", Justification = "Returns the parameter itself.")]

/// <summary>
/// Extensions for <see cref="IReviewConditionsBuilder{TReviewSettings}"/>.
/// </summary>
public static partial class ReviewConditionBuilderExtensions
{
	/// <summary>
	/// The number of completed primary actions must be at least <paramref name="minimumActionCompleted"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder"><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</param>
	/// <param name="minimumActionCompleted">The minimum number of completed actions.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumPrimaryActionsCompleted<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, int minimumActionCompleted)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.PrimaryActionCompletedCount >= minimumActionCompleted)
		);
		return builder;
	}

	/// <summary>
	/// The number of completed secondary actions must be at least <paramref name="minimumActionCompleted"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder"><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</param>
	/// <param name="minimumActionCompleted">The minimum number of completed actions.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumSecondaryActionsCompleted<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, int minimumActionCompleted)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.SecondaryActionCompletedCount >= minimumActionCompleted)
		);
		return builder;
	}

	/// <summary>
	/// The number of times the application has been launched must be at least <paramref name="minimumCount"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder"><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</param>
	/// <param name="minimumCount">The minimum number of times the application has been launched.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumApplicationLaunchCount<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, int minimumCount)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.ApplicationLaunchCount >= minimumCount)
		);
		return builder;
	}

	/// <summary>
	/// The time elapsed since the first application launch must be at least <paramref name="minimumTimeElapsed"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <param name="builder"><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</param>
	/// <param name="minimumTimeElapsed">The minimum time elapsed since the the first application launch.</param>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> MinimumTimeElapsedSinceApplicationFirstLaunch<TReviewSettings>(this IReviewConditionsBuilder<TReviewSettings> builder, TimeSpan minimumTimeElapsed)
		where TReviewSettings : ReviewSettings
	{
		builder.Conditions.Add(new SynchronousReviewCondition<TReviewSettings>(
			(reviewSettings, currentDateTime) => reviewSettings.ApplicationFirstLaunched.HasValue && reviewSettings.ApplicationFirstLaunched.Value + minimumTimeElapsed <= currentDateTime)
		);
		return builder;
	}
}
