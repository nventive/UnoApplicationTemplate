using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Asynchronous implementation of <see cref="IReviewCondition{TReviewSettings}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public sealed class AsynchronousReviewCondition<TReviewSettings> : IReviewCondition<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	private readonly Func<CancellationToken, TReviewSettings, DateTimeOffset, Task<bool>> _condition;

	public AsynchronousReviewCondition(Func<CancellationToken, TReviewSettings, DateTimeOffset, Task<bool>> condition)
	{
		_condition = condition;
	}

	/// <inheritdoc/>
	public async Task<bool> Validate(CancellationToken ct, TReviewSettings currentSettings, DateTimeOffset currentDateTime)
	{
		return await _condition(ct, currentSettings, currentDateTime);
	}
}
