using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;
using DynamicData;
using Uno.Extensions;

namespace ApplicationTemplate.Business;

public sealed class DadJokesService : IDadJokesService, IDisposable
{
	private readonly IApplicationSettingsRepository _applicationSettingsRepository;
	private readonly IDadJokesApiClient _dadJokesApiClient;
	private SourceList<DadJokesQuote> _favouriteQuotes;
	private ReplaySubject<PostTypes> _postType;

	public DadJokesService(IApplicationSettingsRepository applicationSettingsRepository, IDadJokesApiClient dadJokesApiClient)
	{
		_applicationSettingsRepository = applicationSettingsRepository ?? throw new ArgumentNullException(nameof(applicationSettingsRepository));
		_dadJokesApiClient = dadJokesApiClient ?? throw new ArgumentNullException(nameof(dadJokesApiClient));
		_postType = new ReplaySubject<PostTypes>(1);
		_postType.OnNext(PostTypes.Hot);
	}

	public async Task<DadJokesQuote[]> FetchData(CancellationToken ct)
	{
		var pt = await _postType.FirstAsync();

		var postType = pt.ToRedditFilter();

		var response = await _dadJokesApiClient.FetchData(ct, postType);

		var settings = await _applicationSettingsRepository.GetCurrent(ct);

		return response
			.Data
			.Children
			.Safe()
			.Where(d => d.Data.Distinguished != "moderator")
			.Select(d => new DadJokesQuote(d.Data, settings.FavoriteQuotes.ContainsKey(d.Data.Id)))
			.ToArray();
	}

	public ReplaySubject<PostTypes> GetAndObservePostTypeFilter()
	{
		return _postType;
	}

	public void SetPostTypeFilter(PostTypes pt)
	{
		_postType.OnNext(pt);
	}

	public async Task<IObservableList<DadJokesQuote>> GetFavorites(CancellationToken ct)
	{
		var source = await GetFavouriteQuotesSource(ct);
		return source.AsObservableList();
	}

	public async Task SetIsFavorite(CancellationToken ct, DadJokesQuote quote, bool isFavorite)
	{
		ArgumentNullException.ThrowIfNull(quote);

		var settings = await _applicationSettingsRepository.GetCurrent(ct);

		quote = quote with { IsFavorite = isFavorite };

		var updatedFavorites = isFavorite && !settings.FavoriteQuotes.ContainsKey(quote.Id)
			? settings.FavoriteQuotes.Add(quote.Id, quote.ToFavoriteJokeData())
			: settings.FavoriteQuotes.Remove(quote.Id);

		await _applicationSettingsRepository.SetFavoriteQuotes(ct, updatedFavorites);

		var source = await GetFavouriteQuotesSource(ct);

		if (isFavorite)
		{
			if (source.Items.None(q => q.Id == quote.Id))
			{
				source.Add(quote);
			}
		}
		else
		{
			var item = source.Items.FirstOrDefault(q => q.Id == quote.Id);
			source.Remove(item);
		}
	}

	private async Task<SourceList<DadJokesQuote>> GetFavouriteQuotesSource(CancellationToken ct)
	{
		if (_favouriteQuotes == null)
		{
			var settings = await _applicationSettingsRepository.GetCurrent(ct);

			_favouriteQuotes = new SourceList<DadJokesQuote>();
			_favouriteQuotes.AddRange(settings.FavoriteQuotes.Values.Select(favoriteData => new DadJokesQuote(favoriteData)));
		}

		return _favouriteQuotes;
	}

	public void Dispose()
	{
		_favouriteQuotes?.Dispose();
		_postType.Dispose();
	}
}
