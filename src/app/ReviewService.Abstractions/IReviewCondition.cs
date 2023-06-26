using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// A condition used to determine if a review should be requested based on <see cref="ReviewSettings"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public interface IReviewCondition<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	/// <summary>
	/// Validates that the condition is satisfied.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <param name="currentSettings">The persisted <see cref="TReviewSettings"/> tracked by the application.</param>
	/// <param name="currentDateTime">The current date and time.</param>
	/// <returns>If a condition is satisfied.</returns>
	Task<bool> Validate(CancellationToken ct, TReviewSettings currentSettings, DateTimeOffset currentDateTime);
}
