#if WINDOWS
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// Windows implementation of <see cref="IReviewPrompter"/>.
/// </summary>
public sealed class ReviewPrompter : IReviewPrompter
{
	private readonly ILogger _logger;

	public ReviewPrompter(ILogger logger)
	{
		_logger = logger;
	}

	/// <inheritdoc/>
	public Task TryPrompt()
	{
		throw new NotImplementedException();
	}
}
#endif
