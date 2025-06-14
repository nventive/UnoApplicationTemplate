using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Tests;

namespace DadJokes;

public sealed class DadJokesShould : FunctionalTestBase
{
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

	[Fact]
	public async Task ToggleJokeAsFavorite()
	{
		// Arrange
		await this.Login(await this.ReachLoginPage());
		var vm = GetAndAssertActiveViewModel<DadJokesPageViewModel>();
		var jokes = await vm.Jokes.Load(CancellationToken.None);
		jokes.Should().NotBeEmpty();

		var firstJoke = jokes.First();
		var initialFavoriteState = firstJoke.IsFavorite;

		// Act
		await vm.ToggleIsFavorite.Execute(CancellationToken.None, firstJoke);

		// Assert
		firstJoke.IsFavorite.Should().Be(!initialFavoriteState,
			because: "the favorite state should be toggled from its initial value");
	}
}
