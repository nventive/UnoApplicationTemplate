using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Provides ways to validate review prompt conditions using <see cref="IReviewSettingsSource{TReviewPromptSettings}"/> and
/// to prompt user to review the current application using <see cref="IReviewPrompter"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public interface IReviewService<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	/// <summary>
	/// Checks if all review prompt conditions are satisfied and then prompt user to review the current application.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	Task TryRequestReview(CancellationToken ct);

	/// <summary>
	/// Gets if all conditions are satisfied which means that we can prompt user to review the current application.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns>If all conditions are satisfied.</returns>
	Task<bool> GetAreConditionsSatisfied(CancellationToken ct);

	/// <summary>
	/// Updates the persisted <see cref="TReviewSettings"/>.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <param name="updateFunction">Function that returns updated <see cref="TReviewSettings"/>.</param>
	/// <returns><see cref="Task"/>.</returns>
	Task UpdateReviewSettings(CancellationToken ct, Func<TReviewSettings, TReviewSettings> updateFunction);
}
