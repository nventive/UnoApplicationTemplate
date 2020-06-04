using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using DynamicData;

namespace ApplicationTemplate
{
	public class ChuckNorrisFavoritesPageViewModel : ViewModel
	{
		public IDynamicCommand RefreshQuotes => this.GetCommandFromDataLoaderRefresh(Quotes);

		public IDataLoader Quotes => this.GetDataLoader(LoadQuotes);

		public IDynamicCommand ToggleIsFavorite => this.GetCommandFromTask<ChuckNorrisItemViewModel>(async (ct, item) =>
		{
			await this.GetService<IChuckNorrisService>().SetIsFavorite(ct, item.Quote, !item.IsFavorite);

			item.IsFavorite = !item.IsFavorite;
		});

		private SerialDisposable QuotesSubscription => this.GetOrCreateDisposable(() => new SerialDisposable());

		private async Task<ReadOnlyObservableCollection<ChuckNorrisItemViewModel>> LoadQuotes(CancellationToken ct)
		{
			var quotes = await this.GetService<IChuckNorrisService>().GetFavorites(ct);

			QuotesSubscription.Disposable = quotes
				.Connect()
				.Transform(q => this.GetChild(() => new ChuckNorrisItemViewModel(this, q), q.Id))
				.ObserveOn(this.GetService<IDispatcherScheduler>())
				.Bind(out var list)
				.DisposeMany()
				.Subscribe();

			return list;
		}
	}
}
