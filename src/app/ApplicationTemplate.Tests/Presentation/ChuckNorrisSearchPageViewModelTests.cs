using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisSearchPageViewModelTests : NavigationTestsBase
	{
		[Fact]
		public async Task It_Should_EmptyCriterion_NoResults()
		{
			await StartNavigation(DefaultCancellationToken, () => new ChuckNorrisSearchPageViewModel());
			var viewModel = (ChuckNorrisSearchPageViewModel)GetCurrentViewModel();
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			quotes.Should().NotBeNull();
			quotes.Should().BeEmpty();
		}

		[Fact]
		public async Task It_Should_NotEmptyCriterion_Results()
		{
			await StartNavigation(DefaultCancellationToken, () => new ChuckNorrisSearchPageViewModel());
			var viewModel = (ChuckNorrisSearchPageViewModel)GetCurrentViewModel();
			viewModel.SearchTerm = "dog";
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			quotes.Should().NotBeNull();
			quotes.Should().NotBeEmpty();
			quotes.All(q => q.Quote.Value.ToUpperInvariant().Contains("DOG")).Should().BeTrue("All quotes found searching for search term 'dog' should contain 'dog' ");
		}
	}
}
