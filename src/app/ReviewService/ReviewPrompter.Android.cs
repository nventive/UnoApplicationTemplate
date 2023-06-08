#if __ANDROID__
using System;
using System.Threading;
using System.Threading.Tasks;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// Android implementation of <see cref="IReviewPrompter"/>.
/// </summary>
public sealed class ReviewPrompter : IReviewPrompter
{
	private readonly Android.OS.Handler _handler = new(Android.OS.Looper.MainLooper);

	/// <inheritdoc/>
	public Task TryPrompt()
	{
		_handler.Post(() =>
		{
			// TODO.
		});
		throw new NotImplementedException();
	}
}
#endif
