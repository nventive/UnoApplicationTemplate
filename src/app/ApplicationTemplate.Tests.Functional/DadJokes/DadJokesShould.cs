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
}
