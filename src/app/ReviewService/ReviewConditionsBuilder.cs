using System;
using System.Collections.Generic;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// Provides methods to get an empty or default <see cref="ReviewConditionsBuilder{TReviewSettings}"/>.
/// </summary>
public static class ReviewConditionsBuilder
{
	/// <summary>
	/// Creates a builder with no conditions based on <typeparamref name="TReviewSettings"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> Empty<TReviewSettings>()
		where TReviewSettings : ReviewSettings
	{
		return new ReviewConditionsBuilder<TReviewSettings>();
	}

	/// <summary>
	/// Creates a builder with no conditions based on <see cref="ReviewSettings"/>.
	/// </summary>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<ReviewSettings> Empty()
	{
		return new ReviewConditionsBuilder<ReviewSettings>();
	}

	/// <summary>
	/// Creates a default builder with basics review conditions based on <typeparamref name="TReviewSettings"/>.
	/// </summary>
	/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<TReviewSettings> Default<TReviewSettings>()
		where TReviewSettings : ReviewSettings
	{
		return new ReviewConditionsBuilder<TReviewSettings>()
			.MinimumApplicationLaunchCount(3)
			.MinimumTimeElapsedSinceApplicationFirstLaunch(TimeSpan.FromDays(5))
			.MinimumPrimaryActionsCompleted(2);
	}

	/// <summary>
	/// Creates a default builder with basics review conditions based on <see cref="ReviewSettings"/>.
	/// </summary>
	/// <returns><see cref="IReviewConditionsBuilder{TReviewSettings}"/>.</returns>
	public static IReviewConditionsBuilder<ReviewSettings> Default()
	{
		return new ReviewConditionsBuilder<ReviewSettings>()
			.MinimumApplicationLaunchCount(3)
			.MinimumTimeElapsedSinceApplicationFirstLaunch(TimeSpan.FromDays(5))
			.MinimumPrimaryActionsCompleted(2);
	}
}

/// <summary>
/// Implementation of <see cref="IReviewConditionsBuilder{TReviewSettings}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Using same class name, one is generic and the other is not.")]
public sealed class ReviewConditionsBuilder
	<TReviewSettings> : IReviewConditionsBuilder<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	/// <inheritdoc/>
	public IList<IReviewCondition<TReviewSettings>> Conditions { get; } = new List<IReviewCondition<TReviewSettings>>();
}
