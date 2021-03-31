using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisFavoritesPageViewModelTests : NavigationTestsBase
	{
		[Fact]
		public async Task It_Should_Return_No_Results()
		{
			// Arrange
			await NavigateAndClear(DefaultCancellationToken, () => new ChuckNorrisFavoritesPageViewModel());
			var viewModel = (ChuckNorrisFavoritesPageViewModel)GetCurrentViewModel();

			// Act
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken) as ReadOnlyObservableCollection<ChuckNorrisItemViewModel>;

			// Assert
			quotes.Should().BeNullOrEmpty();
		}

		[Fact]
		public async Task It_Should_Return_Favourited()
		{
			// Arrange
			var mockChuckNorrisService = new Mock<IChuckNorrisService>();
			var searchViewModel = new ChuckNorrisSearchPageViewModel();

			searchViewModel.SearchTerm = "dog";
			var searchedQuotes = await searchViewModel.Quotes.Load(DefaultCancellationToken);
			var firstQuoteVm = searchedQuotes.First();

			// Act
			await searchViewModel.ToggleIsFavorite.Execute(firstQuoteVm);

			await NavigateAndClear(DefaultCancellationToken, () => new ChuckNorrisFavoritesPageViewModel());
			var favouritesViewModel = (ChuckNorrisFavoritesPageViewModel)GetCurrentViewModel();

			var favouritedQuotes = await favouritesViewModel.Quotes.Load(DefaultCancellationToken) as ReadOnlyObservableCollection<ChuckNorrisItemViewModel>;

			// ReadOnlyObservableCollection does not load its content directly. Waiting for the CollectionChanged is a workaround to wait for its content to be loaded.
			var tcs = new TaskCompletionSource<Unit>();
			// CollectionChanged is private for ReadOnlyObservableCollection, therefore casting it into INotifyCollectionChanged is a workaround to be able to access this event
			// https://stackoverflow.com/questions/2058176/why-is-readonlyobservablecollection-collectionchanged-not-public
			((INotifyCollectionChanged)favouritedQuotes).CollectionChanged += OnFavouriteCollectionChanged;
			await tcs.Task;
			((INotifyCollectionChanged)favouritedQuotes).CollectionChanged -= OnFavouriteCollectionChanged;

			// Assert
			favouritedQuotes.Should().NotBeNull();
			var results = favouritedQuotes
				.Select(vm => vm.Quote);
			results.Should().Contain(firstQuoteVm.Quote);

			void OnFavouriteCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
			{
				try
				{
					tcs.TrySetResult(Unit.Default);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
			}
		}
	}
}
