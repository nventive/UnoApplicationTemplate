using System;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate.Presentation
{
	public partial class OnboardingPageViewModel : ViewModel
	{
		public OnboardingPageViewModel()
		{
			OnboardingItems = new OnboardingItemViewModel[]
			{
				new OnboardingItemViewModel(this.GetService<IStringLocalizer>()["Onboarding_Content"], "ms-appx:///Assets/Tutorial_FirstScreen_Icon.png"),
				new OnboardingItemViewModel(this.GetService<IStringLocalizer>()["Onboarding_Content"], "ms-appx:///Assets/Tutorial_SecondScreen_Icon.png"),
				new OnboardingItemViewModel(this.GetService<IStringLocalizer>()["Onboarding_Content"], "ms-appx:///Assets/Tutorial_ThirdScreen_Icon.png"),
			};
		}

		public IDynamicCommand NavigateToLogin => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<ISectionsNavigator>().NavigateAndClear(ct, () => new LoginPageViewModel(isFirstLogin: true));
			await this.GetService<IApplicationSettingsService>().CompleteOnboarding(ct);
		});

		public OnboardingItemViewModel[] OnboardingItems
		{
			get => this.Get<OnboardingItemViewModel[]>();
			set => this.Set(value);
		}

		public static implicit operator OnboardingPageViewModel(OnboardingItemViewModel v)
		{
			throw new NotImplementedException();
		}
	}
}
