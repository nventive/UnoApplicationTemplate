using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisFavoritesPageViewModelTests : NavigationTestsBase
	{
		[Fact]
		public async Task It_Should_Return_No_Results()
		{
			await StartNavigation(DefaultCancellationToken, () => new ChuckNorrisFavoritesPageViewModel());
			var viewModel = (ChuckNorrisFavoritesPageViewModel)GetCurrentViewModel();
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken) as ReadOnlyObservableCollection<ChuckNorrisItemViewModel>;

			quotes.Should().BeNullOrEmpty();
		}

		[Fact]
		public async Task It_Should_Return_Favourited()
		{
			await StartNavigation(DefaultCancellationToken, () => new ChuckNorrisSearchPageViewModel());
			var searchViewModel = (ChuckNorrisSearchPageViewModel)GetCurrentViewModel();
			searchViewModel.SearchTerm = "dog";
			var searchedQuotes = await searchViewModel.Quotes.Load(DefaultCancellationToken);
			var firstQuote = searchedQuotes.First();

			await StartNavigation(DefaultCancellationToken, () => new ChuckNorrisFavoritesPageViewModel());
			var favouritesViewModel = (ChuckNorrisFavoritesPageViewModel)GetCurrentViewModel();
			await favouritesViewModel.ToggleIsFavorite.Execute(firstQuote);
			var favouritedQuotes = await favouritesViewModel.Quotes.Load(DefaultCancellationToken) as ReadOnlyObservableCollection<ChuckNorrisItemViewModel>;

			favouritedQuotes.Should().NotBeNull();
			favouritedQuotes.Should().Contain(firstQuote);
		}
	}
}
