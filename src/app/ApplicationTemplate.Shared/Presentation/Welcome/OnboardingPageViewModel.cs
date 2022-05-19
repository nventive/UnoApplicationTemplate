using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation
{
	public partial class OnboardingPageViewModel : ViewModel
	{
		public OnboardingPageViewModel(bool isFromSettingsPage = false)
		{
			IsFromSettingsPage = isFromSettingsPage;
		}

		private bool IsFromSettingsPage { get; set; }

		public IDynamicCommand NavigateToNextPage => this.GetCommandFromTask(async ct =>
		{
			if (IsFromSettingsPage)
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
				await NavigateToLogin(ct);
			}
		});

		public async Task NavigateToLogin(CancellationToken ct)
		{
			await this.GetService<ISectionsNavigator>().NavigateAndClear(ct, () => new LoginPageViewModel(isFirstLogin: true));
			await this.GetService<IApplicationSettingsService>().CompleteOnboarding(ct);
		}

		public OnboardingItemViewModel[] OnboardingItems { get; } = new[]
		{
			new OnboardingItemViewModel("Get your shot of good humor everyday. Read the latest and greatest dad jokes!", "ms-appx:///Assets/Tutorial_FirstScreen_Icon.png"),
			new OnboardingItemViewModel("Get your shot of good humor everyday. Read the latest and greatest dad jokes!", "ms-appx:///Assets/Tutorial_SecondScreen_Icon.png"),
			new OnboardingItemViewModel("Get your shot of good humor everyday. Read the latest and greatest dad jokes!", "ms-appx:///Assets/Tutorial_ThirdScreen_Icon.png"),
		};

		public static implicit operator OnboardingPageViewModel(OnboardingItemViewModel v)
		{
			throw new NotImplementedException();
		}
	}
}
