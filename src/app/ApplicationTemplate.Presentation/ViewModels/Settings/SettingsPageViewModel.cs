using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using MessageDialogService;
using Microsoft.Extensions.Localization;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed partial class SettingsPageViewModel : ViewModel
{
	[Inject] private ILauncherService _browserService;
	[Inject] private IStringLocalizer _stringLocalizer;
	[Inject] private ISectionsNavigator _sectionsNavigator;
	[Inject] private IAuthenticationService _authenticationService;

	public string VersionNumber => this.Get(GetVersionNumber);

	public IDataLoader<UserProfile> UserProfile => this.GetDataLoader(GetUserProfile, db => db
		.TriggerFromObservable(_authenticationService.GetAndObserveIsAuthenticated().Skip(1))
	);

	public IDynamicCommand Logout => this.GetCommandFromTask(async ct =>
	{
		var logout = await this.GetService<IMessageDialogService>().ShowMessage(ct, mb => mb
			.TitleResource("Logout_Title")
			.ContentResource("Logout_Content")
			.CancelCommand()
			.AcceptCommand("Logout_Confirm")
		);

		if (logout == MessageDialogResult.Accept)
		{
			await _authenticationService.Logout(ct);
			await _sectionsNavigator.SetActiveSection(ct, "Login", () => new LoginPageViewModel(isFirstLogin: false), returnToRoot: true);
		}
	});

	public IDynamicCommand NavigateToResetPasswordPage => this.GetCommandFromTask(async ct =>
	{
		await _sectionsNavigator.Navigate(ct, () => new ResetPasswordPageViewModel());
	});

	public IDynamicCommand NavigateToDiagnosticsPage => this.GetCommandFromTask(async ct =>
	{
		IsModalActive = true;
		await _sectionsNavigator.OpenModal(ct, () => new DiagnosticsPageViewModel());
	});

	public IDynamicCommand NavigateToOnboardingPage => this.GetCommandFromTask(async ct =>
	{
		await _sectionsNavigator.Navigate(ct, () => new OnboardingPageViewModel(isFromSettingsPage: true));
	});

	public IDynamicCommand NavigateToPrivacyPolicyPage => this.GetCommandFromTask(async ct =>
	{
		var url = _stringLocalizer["PrivacyPolicyUrl"];

		await _browserService.Launch(new Uri(url));
	});

	public IDynamicCommand NavigateToTermsAndConditionsPage => this.GetCommandFromTask(async ct =>
	{
		var url = _stringLocalizer["TermsAndConditionsUrl"];

		await _browserService.Launch(new Uri(url));
	});

	private async Task<UserProfile> GetUserProfile(CancellationToken ct)
	{
		return await this.GetService<IUserProfileService>().GetCurrent(ct);
	}

	private string GetVersionNumber()
	{
		return this.GetService<IVersionProvider>().GetFullVersionString();
	}
}
