using System.Threading;
using System.Threading.Tasks;
using Nventive.Persistence;
using ReviewService.Abstractions;

namespace ApplicationTemplate.Client;

/// <summary>
/// Implementation of <see cref="IReviewSettingsSource{TReviewSettings}"/>.
/// </summary>
public sealed class ReviewSettingsSource : IReviewSettingsSource<ReviewSettingsCustom>
{
	private readonly IObservableDataPersister<ReviewSettingsCustom> _dataPersister;

	public ReviewSettingsSource(IObservableDataPersister<ReviewSettingsCustom> dataPersister)
	{
		_dataPersister = dataPersister;
	}

	/// <inheritdoc/>
	public async Task<ReviewSettingsCustom> Read(CancellationToken ct)
	{
		var result = await _dataPersister.Load(ct, defaultValue: default);

		return result;
	}

	/// <inheritdoc/>
	public async Task Write(CancellationToken ct, ReviewSettingsCustom reviewSettings)
	{
		await _dataPersister.Update(ct, context =>
		{
			context.Commit(reviewSettings);
		});
	}
}
