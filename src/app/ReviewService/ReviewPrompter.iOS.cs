#if __IOS__
using System;
using System.Threading;
using System.Threading.Tasks;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// iOS implementation of <see cref="IReviewPrompter"/>.
/// </summary>
public sealed class ReviewPrompter : IReviewPrompter
{
	/// <inheritdoc/>
	public Task TryPrompt()
	{
		Foundation.NSRunLoop.Main.BeginInvokeOnMainThread(() =>
		{
			// TODO.
		});
		throw new NotImplementedException();
	}
}
#endif
