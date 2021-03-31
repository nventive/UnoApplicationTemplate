using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using DynamicData;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class ChuckNorrisSearchPageViewModelShould : NavigationTestsBase
	{
		private void MockingGetFavorites(Mock<IChuckNorrisService> mock)
		{
			mock
				.Setup(m => m.GetFavorites(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new SourceList<ChuckNorrisQuote>().AsObservableList());
		}

		// Basic class configuration
		private void ChuckNorrisSearchPageViewModelShould_SpecialConfiguration(IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				// This will replace the actual implementation of IApplicationSettingsService with a mocked version.
				ReplaceWithMock<IChuckNorrisService>(services, mock =>
				{
					MockingGetFavorites(mock);
				});
			});
		}

		[Theory]
		[InlineData("")]
		[InlineData("a")]
		[InlineData("aa")]
		public async Task ReturnEmptyCriterion_WhenProvidedSearchTermIsTooShort(string searchTerm)
		{
			// Arrange
			var viewModel = new ChuckNorrisSearchPageViewModel();
			ConfigurationSetup(ChuckNorrisSearchPageViewModelShould_SpecialConfiguration);

			// Act
			viewModel.SearchTerm = searchTerm;
			var quotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			quotes.Should().BeEmpty();
		}

		[Fact]
		public async Task UpdateSearchTermProperty()
		{
			// Arrange
			var anything = "Anything";
			var searchPageViewModelMock = new Mock<ChuckNorrisSearchPageViewModel>();

			// Act
			searchPageViewModelMock.Object.SearchTerm = anything;

			// Assert
			searchPageViewModelMock.Object.SearchTerm
				.Should().Be(anything);
		}

		private void CheckForSearchMethod_WhenQuotesAreLoading_SpecialConfiguration(IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				// This will replace the actual implementation of IApplicationSettingsService with a mocked version.
				ReplaceWithMock<IChuckNorrisService>(services, mock =>
				{
					mock
						.Setup(m => m.Search(It.IsAny<CancellationToken>(), It.IsAny<string>()))
						.ReturnsAsync(new ChuckNorrisQuote[]
						{
							new ChuckNorrisQuote(new ChuckNorrisData.Builder().WithId("0").WithValue("Something something dog"), false),
							new ChuckNorrisQuote(new ChuckNorrisData.Builder().WithId("1203").WithValue("Dog something"), false)
						});

					MockingGetFavorites(mock);
				});
			});
		}

		[Fact]
		public async Task CheckForSearchMethod_WhenQuotesAreLoading()
		{
			// Arrange
			ConfigurationSetup(CheckForSearchMethod_WhenQuotesAreLoading_SpecialConfiguration);
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

		private void GiveDifferentResults_ForMultipleSearchMethodCall_SpecialConfiguration(IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				// This will replace the actual implementation of IApplicationSettingsService with a mocked version.
				ReplaceWithMock<IChuckNorrisService>(services, mock =>
				{
					mock
						.Setup(m => m.Search(It.IsAny<CancellationToken>(), "dog"))
						.ReturnsAsync(new ChuckNorrisQuote[]
						{
							new ChuckNorrisQuote(new ChuckNorrisData.Builder().WithId("1"), false),
							new ChuckNorrisQuote(new ChuckNorrisData.Builder().WithId("3"), false),
							new ChuckNorrisQuote(new ChuckNorrisData.Builder().WithId("1204"), false)
						});

					mock
						.Setup(m => m.Search(It.IsAny<CancellationToken>(), "cat"))
						.ReturnsAsync(new ChuckNorrisQuote[]
						{
							new ChuckNorrisQuote(new ChuckNorrisData.Builder().WithId("1"), false)
						});

					MockingGetFavorites(mock);
				});
			});
		}

		[Fact]
		public async Task GiveDifferentResults_ForMultipleSearchMethodCall()
		{
			// Arrange
			ConfigurationSetup(GiveDifferentResults_ForMultipleSearchMethodCall_SpecialConfiguration);

			var viewModel = new ChuckNorrisSearchPageViewModel();

			// Act
			viewModel.SearchTerm = "dog";
			var firstQuotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			viewModel.SearchTerm = "cat";
			var secondQuotes = await viewModel.Quotes.Load(DefaultCancellationToken);

			// Assert
			using (new AssertionScope())
			{
				firstQuotes.Length
					.Should().Be(3);

				secondQuotes.Length
					.Should().Be(1);
			}
		}
	}
}
