using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Presentation;
using DynamicData;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class DadJokesFilterPageViewModelShould : NavigationTestsBase
	{
		private void MockingGetFavorites(Mock<IDadJokesService> mock)
		{
			mock
				.Setup(m => m.GetFavorites(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new SourceList<DadJokesQuote>().AsObservableList());
		}

		// Basic class configuration
		private void DadJokesFilterPageViewModelShould_Configuration(IHostBuilder host)
		{
			host.ConfigureServices(services =>
			{
				// This will replace the actual implementation of IApplicationSettingsService with a mocked version.
				ReplaceWithMock<IDadJokesService>(services, mock =>
				{
					MockingGetFavorites(mock);
				});
			});
		}

		[Theory]
		[InlineData(PostTypes.Hot)]
		[InlineData(PostTypes.New)]
		[InlineData(PostTypes.Rising)]
		public async Task ReturnPosts_WhenProvidedPostType(PostTypes postType)
		{
			// Arrange
			var dadJokesVM = (DadJokesPageViewModel)await Navigate(DefaultCancellationToken, () => new DadJokesPageViewModel());

			var filtersVM = await AssertNavigateTo<DadJokesFiltersPageViewModel>(() => dadJokesVM.NavigateToFilters);

			filtersVM.PostTypeFilter = postType;

			await AssertNavigateTo<DadJokesPageViewModel>(() => filtersVM.FiltersAndNavigate);
		}
	}
}
