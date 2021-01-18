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
			// Arrange
			await NavigateAndClear(DefaultCancellationToken, () => new ChuckNorrisSearchPageViewModel());
			var viewModel = (ChuckNorrisSearchPageViewModel)GetCurrentViewModel();

			// Act
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			quotes.Should().NotBeNull();
			quotes.Should().BeEmpty();
		}

		[Fact]
		public async Task It_Should_NotEmptyCriterion_Results()
		{
			// Arrange
			await NavigateAndClear(DefaultCancellationToken, () => new ChuckNorrisSearchPageViewModel());
			var viewModel = (ChuckNorrisSearchPageViewModel)GetCurrentViewModel();

			// Act
			viewModel.SearchTerm = "dog";
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			quotes.Should().NotBeNull();
			quotes.Should().NotBeEmpty();
			quotes.All(q => q.Quote.Value.ToUpperInvariant().Contains("DOG")).Should().BeTrue("All quotes found searching for search term 'dog' should contain 'dog' ");
		}
	}
}
