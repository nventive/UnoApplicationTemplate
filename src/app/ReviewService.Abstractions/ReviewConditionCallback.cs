using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReviewService.Abstractions;

/// <summary>
///  A condition used to determine if a review should be requested based on <see cref="ReviewSettings"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of the object that we use for tracking.</typeparam>
/// <param name="ct"><see cref="CancellationToken"/>.</param>
/// <param name="reviewSettings">The persisted <see cref="ReviewSettings"/> tracked by the application.</param>
/// <param name="currentDateTime">The current date and time.</param>
/// <returns>If a review should be requested.</returns>
public delegate Task<bool> ReviewConditionCallback<TReviewSettings>(CancellationToken ct, TReviewSettings reviewSettings, DateTimeOffset currentDateTime)
	where TReviewSettings : ReviewSettings;
