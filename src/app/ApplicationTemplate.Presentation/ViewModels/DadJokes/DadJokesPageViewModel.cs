using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using DynamicData;

namespace ApplicationTemplate.Presentation;

public partial class DadJokesPageViewModel : ViewModel
{
	public DadJokesPageViewModel()
	{
		Console.WriteLine("init");
	}

	public IDynamicCommand NavigateToFilters => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new DadJokesFiltersPageViewModel());
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
	});

	private async Task<DadJokesItemViewModel[]> LoadJokes(CancellationToken ct, IDataLoaderRequest request)
	{
		await SetupFavoritesUpdate(ct);
		var quotes = await this.GetService<IDadJokesService>().FetchData(ct);

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
			var favorites = await this.GetService<IDadJokesService>().GetFavorites(ct);

			// Subscribe to the observable list to update the current items.
			var subscription = favorites
				.Connect()
				.Subscribe(UpdateItemViewModels);

			AddDisposable(FavoritesKey, subscription);
		}

		void UpdateItemViewModels(IChangeSet<DadJokesQuote> changeSet)
		{
			var quotesVMs = Jokes.State.Data;
			if (quotesVMs != null && quotesVMs.Any())
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
