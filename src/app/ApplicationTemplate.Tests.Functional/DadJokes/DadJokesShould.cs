using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Tests;
using ApplicationTemplate.Business;  // Added for DadJokesQuote and related types
using ApplicationTemplate.DataAccess;  // Added for IDadJokesService
using DynamicData;  // Added for SourceCache
using Microsoft.Extensions.Hosting;  // Added for IHostBuilder
using NSubstitute;  // Already used in base class, ensures mocking works

namespace DadJokes;

public sealed class DadJokesShould : FunctionalTestBase
{
	// Added: Field to hold the mocked IDadJokesService for use in tests.
	private IDadJokesService MockDadJokesService;

	// Added: Override ConfigureHost to set up the mock for IDadJokesService.
	// This allows controlling service behavior and verifying calls in tests without real API interactions.
	protected override void ConfigureHost(IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureServices(services =>
		{
			ReplaceWithMock(services, out MockDadJokesService);
		});
	}

	[Fact]
	public async Task LoadAListOfJokes()
	{
		// Arrange
		await this.Login(await this.ReachLoginPage());
		var vm = GetAndAssertActiveViewModel<DadJokesPageViewModel>();

		// Act
		var jokes = await vm.Jokes.Load(CancellationToken.None);

		// Assert
		jokes.Should().NotBeEmpty();
	}

	// Added: New test to validate adding a dad joke as a favorite.
	// It verifies that ToggleIsFavorite calls the service and updates the ViewModel state.
	[Fact]
	public async Task AddJokeAsFavorite()
	{
		// Arrange: Set up the mock to return a sample non-favorite joke and an empty favorites cache.
		// This simulates data loading and allows verifying service interactions.
		var sampleQuotes = new[]
		{
			new DadJokesQuote { Id = "1", Content = "Sample Joke", IsFavorite = false }
		};
		MockDadJokesService.FetchData(Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(sampleQuotes.AsEnumerable()));
		var favoritesCache = new SourceCache<DadJokesQuote, string>(q => q.Id);  // Empty cache for no initial favorites.
		MockDadJokesService.GetFavorites(Arg.Any<CancellationToken>())
			.Returns(favoritesCache);

		// Navigate to the Dad Jokes page and load jokes (reusing base class helpers).
		await this.Login(await this.ReachLoginPage());
		var vm = GetAndAssertActiveViewModel<DadJokesPageViewModel>();
		var jokes = await vm.Jokes.Load(CancellationToken.None);
		var jokeToFavorite = jokes.First();  // Select the first joke (assumed non-favorite based on mock).

		// Act: Execute the ToggleIsFavorite command to add the joke as a favorite.
		await vm.ToggleIsFavorite.Execute(jokeToFavorite);

		// Assert: Verify the service was called to set the favorite and the ViewModel updated.
		// This confirms the command interacts with the service and reflects changes in the UI state.
		await MockDadJokesService.Received().SetIsFavorite(
			Arg.Any<CancellationToken>(),
			Arg.Is<DadJokesQuote>(q => q.Id == jokeToFavorite.Quote.Id),  // Matches the selected quote.
			true  // Toggles from false to true.
		);
		jokeToFavorite.IsFavorite.Should().BeTrue();  // Ensures the property updates via the subscription.
	}
}
