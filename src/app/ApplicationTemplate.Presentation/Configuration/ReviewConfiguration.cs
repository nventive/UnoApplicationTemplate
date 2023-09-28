using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using ReviewService;

namespace ApplicationTemplate;

/// <summary>
/// This interface wraps <see cref="IReviewService{TReviewSettings}"/> so that you don't have to repeat the generic parameter everywhere that you would use the review service.
/// In other words, you should use this interface in the app instead of <see cref="IReviewService{TReviewSettings}"/> because it's leaner.
/// </summary>
/// <remarks>
/// If you would change <see cref="ReviewSettings"/> for a custom type, using this interface allows you to minimize any refactoring effort by limiting it to this interface and the associated adapter.
/// </remarks>
public interface IReviewService : IReviewService<ReviewSettings>
{
}

public static class ReviewConfiguration
{
	public static IServiceCollection AddReviewServices(this IServiceCollection services)
	{
		return services
			.AddTransient(s => ReviewConditionsBuilder
				// TODO: Customize conditions of review prompt. See https://github.com/nventive/ReviewService#configure-conditions for more details.
				// The current conditions are for easy debugging and should not be used in production.
				.Empty()
				.MinimumPrimaryActionsCompleted(3)
			)
			.AddSingleton<IReviewPrompter, LoggingReviewPrompter>()
			.AddSingleton<IReviewSettingsSource<ReviewSettings>, DataPersisterReviewSettingsSource<ReviewSettings>>()
			.AddSingleton<IReviewService<ReviewSettings>, ReviewService<ReviewSettings>>()
			.AddSingleton<IReviewService, ReviewServiceAdapter>();
	}

	private sealed class ReviewServiceAdapter : IReviewService
	{
		private readonly IReviewService<ReviewSettings> _reviewService;

		public ReviewServiceAdapter(IReviewService<ReviewSettings> reviewService)
		{
			_reviewService = reviewService;
		}

		public Task<bool> GetAreConditionsSatisfied(CancellationToken ct) => _reviewService.GetAreConditionsSatisfied(ct);

		public Task TryRequestReview(CancellationToken ct) => _reviewService.TryRequestReview(ct);

		public Task UpdateReviewSettings(CancellationToken ct, Func<ReviewSettings, ReviewSettings> updateFunction) => _reviewService.UpdateReviewSettings(ct, updateFunction);
	}
}
