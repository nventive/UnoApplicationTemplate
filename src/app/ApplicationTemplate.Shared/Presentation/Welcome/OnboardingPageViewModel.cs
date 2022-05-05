using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Microsoft.Extensions.Localization;
using ResourceLoader = Windows.ApplicationModel.Resources.ResourceLoader;

namespace ApplicationTemplate.Presentation
{
	public partial class OnboardingPageViewModel : ViewModel
	{
		public OnboardingPageViewModel(bool isFromSettingsPage = false)
		{
			this.IsFromSettingsPage = isFromSettingsPage;
		}

		public bool IsFromSettingsPage
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public IDynamicCommand NavigateToNextPage => this.GetCommandFromTask(async ct =>
		{
			if (this.IsFromSettingsPage)
			{
				// Forward navigation to navigate back to LoginPage
				// Remove LoginPage for navigation stack
				await this.GetService<ISectionsNavigator>().RemovePrevious(ct);
				await this.GetService<ISectionsNavigator>().Navigate(ct, () => new SettingsPageViewModel());
				// Remove OnboardingPage for navigation stack
				await this.GetService<ISectionsNavigator>().RemovePrevious(ct);
			}
			else
			{
				await this.NavigateToLogin(ct);
			}
		});

		public async Task NavigateToLogin(CancellationToken ct)
		{
			await this.GetService<ISectionsNavigator>().Navigate(ct, () => new LoginPageViewModel(isFirstLogin: true));
			await this.GetService<IApplicationSettingsService>().CompleteOnboarding(ct);
		}

		public OnboardingItemViewModel[] OnboardingItems
		{
			get => new[]
			{
				new OnboardingItemViewModel(this.GetService<IStringLocalizer>()["Onboarding_Content"], "ms-appx:///Assets/Tutorial_FirstScreen_Icon.png"),
				new OnboardingItemViewModel(this.GetService<IStringLocalizer>()["Onboarding_Content"], "ms-appx:///Assets/Tutorial_SecondScreen_Icon.png"),
				new OnboardingItemViewModel(this.GetService<IStringLocalizer>()["Onboarding_Content"], "ms-appx:///Assets/Tutorial_ThirdScreen_Icon.png")
			};
		}

		public static implicit operator OnboardingPageViewModel(OnboardingItemViewModel v)
		{
			throw new NotImplementedException();
		}
	}
}
