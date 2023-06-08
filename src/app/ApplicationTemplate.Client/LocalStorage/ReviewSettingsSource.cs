using System;
using System.Threading;
using System.Threading.Tasks;
using ReviewService.Abstractions;

namespace ApplicationTemplate.Client;

/// <summary>
/// Implementation of <see cref="IReviewSettingsSource{TReviewSettings}"/>.
/// </summary>
public sealed class ReviewSettingsSource : IReviewSettingsSource<ReviewSettings>
{
	/// <inheritdoc/>
	public Task<ReviewSettings> Read(CancellationToken ct)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc/>
	public Task Write(CancellationToken ct, ReviewSettings reviewSettings)
	{
		throw new NotImplementedException();
	}
}
