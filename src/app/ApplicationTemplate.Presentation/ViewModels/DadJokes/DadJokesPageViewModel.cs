using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using DynamicData;
using ReviewService;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed partial class DadJokesPageViewModel : ViewModel
{
	[Inject] private IDadJokesService _dadJokesService;
	[Inject] private ISectionsNavigator _sectionsNavigator;

	public IDynamicCommand NavigateToFilters => this.GetCommandFromTask(async ct =>
	{
		await _sectionsNavigator.Navigate(ct, () => new DadJokesFiltersPageViewModel());
	});

	public IDataLoader<DadJokesItemViewModel[]> Jokes => this.GetDataLoader(LoadJokes, b => b
		// Dispose the previous ItemViewModels when Quotes produces new values.
		.DisposePreviousData()
		.TriggerOnNetworkReconnection(this.GetService<IConnectivityProvider>())
		.TriggerFromObservable(this.GetService<IDadJokesService>().GetAndObservePostTypeFilter().Skip(1))
	);

	public IDynamicCommand ToggleIsFavorite => this.GetCommandFromTask<DadJokesItemViewModel>(async (ct, item) =>
	{
		await this.GetService<IDadJokesService>().SetIsFavorite(ct, item.Quote, !item.IsFavorite);
		await this.GetService<IReviewService>().TrackPrimaryActionCompleted(ct);
	});

	private async Task<DadJokesItemViewModel[]> LoadJokes(CancellationToken ct, IDataLoaderRequest request)
	{
		await SetupFavoritesUpdate(ct);
		var quotes = await _dadJokesService.FetchData(ct);

		return quotes
			.Select(q => this.GetChild(() => new DadJokesItemViewModel(this, q), q.Id))
			.ToArray();
	}

	private async Task SetupFavoritesUpdate(CancellationToken ct)
	{
		const string FavoritesKey = "FavoritesSubscription";

		if (!TryGetDisposable(FavoritesKey, out var _))
		{
			// Get the observable list of favorites.
			var favorites = await _dadJokesService.GetFavorites(ct);

			// Subscribe to the observable list to update the current items.
			var subscription = favorites
				.Connect()
				.Subscribe(UpdateItemViewModels);

			AddDisposable(FavoritesKey, subscription);
		}

		void UpdateItemViewModels(IChangeSet<DadJokesQuote> changeSet)
		{
			var quotesVMs = Jokes.State.Data;
			if (quotesVMs != null && quotesVMs.Length != 0)
			{
				var addedItems = changeSet.GetAddedItems();
				var removedItems = changeSet.GetRemovedItems();

				foreach (var quoteVM in quotesVMs)
				{
					if (addedItems.Any(a => a.Id == quoteVM.Quote.Id))
					{
						quoteVM.IsFavorite = true;
					}
					if (removedItems.Any(r => r.Id == quoteVM.Quote.Id))
					{
						quoteVM.IsFavorite = false;
					}
				}
			}
		}
	}
}
