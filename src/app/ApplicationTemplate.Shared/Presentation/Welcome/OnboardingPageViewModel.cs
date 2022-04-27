using System;
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
		public IDynamicCommand NavigateToLogin => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<ISectionsNavigator>().NavigateAndClear(ct, () => new LoginPageViewModel(isFirstLogin: true));
			await this.GetService<IApplicationSettingsService>().CompleteOnboarding(ct);
		});

		public OnboardingItemViewModel[] OnboardingItems { get; } = new OnboardingItemViewModel[]
		{
#if __ANDROID__ || __IOS__
			new OnboardingItemViewModel(ResourceLoader.GetForViewIndependentUse().GetString("Onboarding_Content"), "ms-appx:///Assets/Tutorial_FirstScreen_Icon.png"),
			new OnboardingItemViewModel(ResourceLoader.GetForViewIndependentUse().GetString("Onboarding_Content"), "ms-appx:///Assets/Tutorial_SecondScreen_Icon.png"),
			new OnboardingItemViewModel(ResourceLoader.GetForViewIndependentUse().GetString("Onboarding_Content"), "ms-appx:///Assets/Tutorial_ThirdScreen_Icon.png"),
#endif
#if WINDOWS_UWP
			new OnboardingItemViewModel("Get your shot of good humor everyday. Read the latest and greatest dad jokes!", "ms-appx:///Assets/Tutorial_FirstScreen_Icon.png"),
			new OnboardingItemViewModel("Get your shot of good humor everyday. Read the latest and greatest dad jokes!", "ms-appx:///Assets/Tutorial_SecondScreen_Icon.png"),
			new OnboardingItemViewModel("Get your shot of good humor everyday. Read the latest and greatest dad jokes!", "ms-appx:///Assets/Tutorial_ThirdScreen_Icon.png"),
#endif
		};

		public static implicit operator OnboardingPageViewModel(OnboardingItemViewModel v)
		{
			throw new NotImplementedException();
		}
	}
}
