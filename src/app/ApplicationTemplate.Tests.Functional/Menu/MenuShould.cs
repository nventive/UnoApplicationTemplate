using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationTemplate.Tests;

namespace Menu;

/// <summary>
/// Tests navigation using the menu.
/// </summary>
public class MenuShould : FunctionalTestBase
{
	[Fact]
	public async Task OpenThePostsSection()
	{
		// Arrange
		await this.Login(await this.ReachLoginPage());

		// Act
		await Menu.ShowPostsSection.Execute();

		// Assert
		ActiveViewModel.Should().BeOfType<PostsPageViewModel>();
	}

	[Fact]
	public async Task OpenTheDadJokesSection()
	{
		// Arrange
		await this.Login(await this.ReachLoginPage());
		await Menu.ShowPostsSection.Execute();

		// Act
		await Menu.ShowHomeSection.Execute();

		// Assert
		ActiveViewModel.Should().BeOfType<DadJokesPageViewModel>();
	}

	[Fact]
	public async Task OpenTheSettingsSection()
	{
		// Arrange
		await this.Login(await this.ReachLoginPage());

		// Act
		await Menu.ShowSettingsSection.Execute();

		// Assert
		ActiveViewModel.Should().BeOfType<SettingsPageViewModel>();
	}
}
