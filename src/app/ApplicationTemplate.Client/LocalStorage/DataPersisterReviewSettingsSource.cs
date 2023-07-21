using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nventive.Persistence;
using ReviewService;

namespace ApplicationTemplate.Client;

/// <summary>
/// Implementation of <see cref="IReviewSettingsSource{TReviewSettings}"/> using <see cref="IDataPersister{T}"/>.
/// </summary>
/// <typeparam name="TReviewSettings">The type of review settings.</typeparam>
public sealed class DataPersisterReviewSettingsSource<TReviewSettings> : IReviewSettingsSource<TReviewSettings>
	where TReviewSettings : ReviewSettings, new()
{
	private readonly IDataPersister<TReviewSettings> _dataPersister;

	public DataPersisterReviewSettingsSource(IDataPersister<TReviewSettings> dataPersister)
	{
		_dataPersister = dataPersister;
	}

	public async Task<TReviewSettings> Read(CancellationToken ct)
	{
		return await _dataPersister.Load(ct, defaultValue: new TReviewSettings());
	}

	public async Task Write(CancellationToken ct, TReviewSettings reviewSettings)
	{
		await _dataPersister.Update(ct, context =>
		{
			context.Commit(reviewSettings);
		});
	}
}
