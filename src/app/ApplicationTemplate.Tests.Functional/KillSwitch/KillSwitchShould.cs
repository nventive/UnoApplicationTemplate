using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using ApplicationTemplate.DataAccess;
using ApplicationTemplate.Tests;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace KillSwitch;

/// <summary>
/// Tests for the kill switch flow.
/// </summary>
public sealed class KillSwitchShould : FunctionalTestBase
{
	private IKillSwitchRepository _killSwitchRepository;
	private Subject<bool> _killSwitchActivatedSubject = new Subject<bool>();

	/// <summary>
	/// Tests that the kill switch page is shown when the kill switch is activated.
	/// </summary>
	[Fact]
	public async Task NavigateToKillSwitchPageWhenTheKillSwitchIsActivated()
	{
		// Arrange
		var vm = await this.ReachLoginPage();

		// Act
		_killSwitchActivatedSubject.OnNext(true);

		await WaitForNavigation(viewModelType: typeof(KillSwitchPageViewModel), isDestination: true);

		// Assert
		ActiveViewModel.Should().BeOfType<KillSwitchPageViewModel>();
	}

	/// <summary>
	/// Tests that you cannot back out of the the kill switch page when the kill switch is activated.
	/// </summary>
	[Fact]
	public async Task DisableTheBackButton()
	{
		// Arrange
		var vm = await this.ReachLoginPage();
		var sectionNavigator = GetService<ISectionsNavigator>();

		// Act
		_killSwitchActivatedSubject.OnNext(true);

		await WaitForNavigation(viewModelType: typeof(KillSwitchPageViewModel), isDestination: true);

		// Navigate back using the hardware button.
		NavigateBackUsingHardwareButton();

		// Waits for the navigation to be completed, we need the StartWith in case the navigation already finished before we started observing.
		await sectionNavigator.ObserveCurrentState()
			.StartWith(sectionNavigator.State)
			.Where(x => x.LastRequestState != NavigatorRequestState.Processing)
			.FirstAsync();

		// Assert
		ActiveViewModel.Should().BeOfType<KillSwitchPageViewModel>();
	}

	/// <summary>
	/// Tests that you are no longer in the killswitch page when the kill switch is deactivated.
	/// </summary>
	[Fact]
	public async Task NavigateBackWhenTheKillSwitchIsDeactivated()
	{
		// Arrange
		var vm = await this.ReachLoginPage();

		// Act
		_killSwitchActivatedSubject.OnNext(true);

		await WaitForNavigation(viewModelType: typeof(KillSwitchPageViewModel), isDestination: true);

		_killSwitchActivatedSubject.OnNext(false);

		await WaitForNavigation(viewModelType: typeof(KillSwitchPageViewModel), isDestination: false);

		// Assert
		ActiveViewModel.Should().NotBeOfType<KillSwitchPageViewModel>();
	}

	/// <inheritdoc />
	protected override void ConfigureHost(IHostBuilder hostBuilder)
	{
		base.ConfigureHost(hostBuilder);
		hostBuilder.ConfigureServices(serviceCollection =>
		{
			ReplaceWithMock<IKillSwitchRepository>(serviceCollection, out _killSwitchRepository);
			_killSwitchRepository.ObserveKillSwitchActivation().Returns(_killSwitchActivatedSubject);
		});
	}
}
