#if WINDOWS
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ReviewService.Abstractions;

namespace ReviewService;

/// <summary>
/// Windows implementation of <see cref="IReviewPrompter"/>.
/// </summary>
public class ReviewPrompter : IReviewPrompter
{
	private readonly ILogger _logger;

	public ReviewPrompter(ILogger<ReviewPrompter> logger)
	{
		_logger = logger ?? NullLogger<ReviewPrompter>.Instance;
	}

	/// <inheritdoc/>
	public Task TryPrompt()
	{
		_logger.LogWarning("Prompting for a review is not implemented on windows.");

		return Task.CompletedTask;
	}
}
#endif
