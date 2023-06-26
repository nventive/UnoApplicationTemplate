using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
/// Synchronous implementation of <see cref="IReviewCondition{TReviewSettings}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
public sealed class SynchronousReviewCondition<TReviewSettings> : IReviewCondition<TReviewSettings>
	where TReviewSettings : ReviewSettings
{
	private readonly Func<TReviewSettings, DateTimeOffset, bool> _condition;

	public SynchronousReviewCondition(Func<TReviewSettings, DateTimeOffset, bool> condition)
	{
		_condition = condition;
	}

	/// <inheritdoc/>
	public Task<bool> Validate(CancellationToken ct, TReviewSettings currentSettings, DateTimeOffset currentDateTime)
	{
		return Task.FromResult(_condition(currentSettings, currentDateTime));
	}
}
