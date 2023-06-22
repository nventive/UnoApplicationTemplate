#if __IOS__
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Plugin.StoreReview;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// iOS implementation of <see cref="IReviewPrompter"/>.
/// </summary>
public sealed class ReviewPrompter : IReviewPrompter
{
	private readonly ILogger _logger;

	public ReviewPrompter(ILogger<ReviewPrompter> logger)
	{
		_logger = logger ?? NullLogger<ReviewPrompter>.Instance;
	}

	/// <inheritdoc/>
	public Task TryPrompt()
	{
		var tcs = new TaskCompletionSource();

		Foundation.NSRunLoop.Main.BeginInvokeOnMainThread(async () =>
		{
			try
			{
				_logger.LogDebug("Trying to prompt the user for a review.");

				await CrossStoreReview.Current.RequestReview(false);

				_logger.LogInformation("Prompted the user for a review.");

				tcs.SetResult();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Failed to prompt the user for a review.");
				tcs.SetException(e);
			}
		});

		return tcs.Task;
	}
}
#endif
