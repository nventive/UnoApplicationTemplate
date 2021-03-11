using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DynamicData;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisSearchPageViewModelTests : NavigationTestsBase
	{
		[Fact]
		public async Task It_Should_EmptyCriterion_NoResults()
		{
			// Arrange
			var mockChuckNorrisService = new Mock<IChuckNorrisService>();
			var viewModel = new ChuckNorrisSearchPageViewModel();

			// Act
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			using (new AssertionScope())
			{
				quotes.Should().NotBeNull();
				quotes.Should().BeEmpty();
			}
		}

		[Fact]
		public async Task It_Should_NotEmptyCriterion_Results()
		{
			// Arrange
			var mockChuckNorrisService = new Mock<IChuckNorrisService>();
			var viewModel = new ChuckNorrisSearchPageViewModel();

			// Act
			viewModel.SearchTerm = "dog";
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			using (new AssertionScope())
			{
				quotes.Should()
					.NotBeNull()
					.And
					.NotBeEmpty();

				quotes.All(q => q.Quote.Value.ToUpperInvariant().Contains("DOG"))
					.Should().BeTrue("All quotes found searching for search term 'dog' should contain 'dog' ");
			}
		}

		[Theory]
		[InlineData("")]
		[InlineData("a")]
		[InlineData("aa")]
		public async Task DisplayNothingIfSearchIsNotValid(string searchTerm)
		{
			// Arrange
			var mockChuckNorrisService = new Mock<IChuckNorrisService>();
			var viewModel = new ChuckNorrisSearchPageViewModel();

			// Act
			viewModel.SearchTerm = searchTerm;
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			quotes.Length
				.Should().Be(0);
		}

		[Fact]
		public async Task UpdateSearchTermProperty()
		{
			// Arrange
			var anything = "Anything";
			var mockChuckNorrisService = new Mock<IChuckNorrisService>();
			var searchPageViewModelMock = new Mock<ChuckNorrisSearchPageViewModel>(mockChuckNorrisService.Object);
			searchPageViewModelMock
				.SetupSet(vm => vm.SearchTerm = anything)
				.Verifiable();

			// Act
			searchPageViewModelMock.Object.SearchTerm = anything;

			// Assert
			searchPageViewModelMock
				.Verify();
		}

		/// <summary>
		/// Verify that the Search method from ChuckNorrisService is:
		/// - Exactly called once if the search term is valid.
		/// - Never called if not.
		/// </summary>
		/// <param name="expectedCallCount">Number of times the Search method must be called.</param>
		/// <param name="searchTerm">The term used for the search.</param>
		/// <param name="chuckNorrisServiceMock">The <see cref="IChuckNorrisService"/> used for the search (mocked).</param>
		/// <returns><see cref="Task"/>.</returns>
		[Theory]
		[InlineAutoData(1, "test")]
		[InlineAutoData(0, "")]
		public async Task CheckForSearchMethodWhenQuotesAreLoading(
			int expectedCallCount,
			string searchTerm,
			Mock<IChuckNorrisService> chuckNorrisServiceMock
		)
		{
			// Arrange
			chuckNorrisServiceMock
				.Setup(s => s.Search(It.IsAny<CancellationToken>(), It.IsAny<string>()))
				.ReturnsAsync(Array.Empty<ChuckNorrisQuote>());

			chuckNorrisServiceMock
				.Setup(s => s.GetFavorites(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new SourceList<ChuckNorrisQuote>().AsObservableList());

			var searchPageViewModel = new ChuckNorrisSearchPageViewModel();

			// Act
			searchPageViewModel.SearchTerm = searchTerm;
			await searchPageViewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			chuckNorrisServiceMock
				.Verify(s =>
					s.Search(It.IsAny<CancellationToken>(), searchPageViewModel.SearchTerm),
					Times.Exactly(expectedCallCount)
				);
		}
	}
}
