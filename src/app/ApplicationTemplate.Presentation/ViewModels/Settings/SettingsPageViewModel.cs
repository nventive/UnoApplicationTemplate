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
	[Inject] private IBrowserService _browserService;

	/*
	public string VersionNumber => this.Get(GetVersionNumber);
	*/

	public IDataLoader<UserProfile> UserProfile => this.GetDataLoader(GetUserProfile, db => db
		.TriggerFromObservable(this.GetService<IAuthenticationService>().GetAndObserveIsAuthenticated().Skip(1))
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
			await this.GetService<IAuthenticationService>().Logout(ct);
			await this.GetService<ISectionsNavigator>().SetActiveSection(ct, "Login", () => new LoginPageViewModel(isFirstLogin: false), returnToRoot: true);
		}
	});

	public IDynamicCommand NavigateToResetPasswordPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new ResetPasswordPageViewModel());
	});

	public IDynamicCommand NavigateToDiagnosticsPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().OpenModal(ct, () => new DiagnosticsPageViewModel());
	});

	public IDynamicCommand NavigateToOnboardingPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new OnboardingPageViewModel(isFromSettingsPage: true));
	});

	public IDynamicCommand NavigateToPrivacyPolicyPage => this.GetCommandFromTask(async ct =>
	{
		var url = this.GetService<IStringLocalizer>()["PrivacyPolicyUrl"];

		await _browserService.OpenAsync(new Uri(url));
	});

	public IDynamicCommand NavigateToTermsAndConditionsPage => this.GetCommandFromTask(async ct =>
	{
		var url = this.GetService<IStringLocalizer>()["TermsAndConditionsUrl"];

		await _browserService.OpenAsync(new Uri(url));
	});

	private async Task<UserProfile> GetUserProfile(CancellationToken ct)
	{
		return await this.GetService<IUserProfileService>().GetCurrent(ct);
	}

	/*
	private string GetVersionNumber()
	{
		return this.GetService<IAppInfo>().VersionString;
	}
	*/
}
