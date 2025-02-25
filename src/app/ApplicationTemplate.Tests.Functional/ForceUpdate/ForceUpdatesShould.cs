using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate.Tests;

/// <summary>
/// Tests for the forced update flow.
/// </summary>
public sealed class ForceUpdatesShould : FunctionalTestBase
{
	/// <summary>
	/// Tests that the force update page is shown when the update required event is raised.
	/// </summary>
	[Fact]
	public async Task RedirectTheAppToForceUpdatePage()
	{
		// Arrange
		var vm = await this.ReachLoginPage();
		var sectionNavigator = GetService<ISectionsNavigator>();
		var minimumVersionProvider = GetService<IMinimumVersionProvider>();

		// Act
		// This will raise the update required event.
		minimumVersionProvider.CheckMinimumVersion();

		await WaitForNavigation(viewModelType: typeof(ForcedUpdatePageViewModel), isDestination: true);

		// Assert
		ActiveViewModel.Should().BeOfType<ForcedUpdatePageViewModel>();
	}

	/// <summary>
	/// Tests that you are still in the forced update page even after you pressed on the back button.
	/// </summary>
	[Fact]
	public async Task DisableTheBackButton()
	{
		// Arrange
		var vm = await this.ReachLoginPage();
		var sectionNavigator = GetService<ISectionsNavigator>();
		var minimumVersionProvider = GetService<IMinimumVersionProvider>();

		// Act
		// This will raise the update required event.
		minimumVersionProvider.CheckMinimumVersion();

		await WaitForNavigation(viewModelType: typeof(ForcedUpdatePageViewModel), isDestination: true);

		NavigateBackUsingHardwareButton();

		await sectionNavigator.ObserveCurrentState().StartWith(sectionNavigator.State).Where(x => x.LastRequestState != NavigatorRequestState.Processing).FirstAsync();

		// Assert
		ActiveViewModel.Should().BeOfType<ForcedUpdatePageViewModel>();
	}
}
