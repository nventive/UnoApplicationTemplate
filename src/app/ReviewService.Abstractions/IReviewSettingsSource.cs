using System.Threading;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Holds the review prompt settings used for prompt conditions.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the persisted object.</typeparam>
public interface IReviewSettingsSource<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	/// <summary>
	/// Gets the current <see cref="ReviewSettings"/>.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <returns>The current <see cref="ReviewSettings"/>.</returns>
	Task<TReviewSettings> Read(CancellationToken ct);

	/// <summary>
	/// Updates the review prompt settings.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/>.</param>
	/// <param name="reviewSettings">Updated review prompt settings.</param>
	/// <returns><see cref="Task"/>.</returns>
	Task Write(CancellationToken ct, TReviewSettings reviewSettings);
}
