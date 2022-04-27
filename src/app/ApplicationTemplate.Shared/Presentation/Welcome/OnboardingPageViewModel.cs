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

		public OnboardingItemViewModel[] OnboardingItems { get; } = new[]
		{
			new OnboardingItemViewModel(ResourceLoader.GetForViewIndependentUse().GetString("Onboarding_Content"), "ms-appx:///Assets/Tutorial_FirstScreen_Icon.png"),
			new OnboardingItemViewModel(ResourceLoader.GetForViewIndependentUse().GetString("Onboarding_Content"), "ms-appx:///Assets/Tutorial_SecondScreen_Icon.png"),
			new OnboardingItemViewModel(ResourceLoader.GetForViewIndependentUse().GetString("Onboarding_Content"), "ms-appx:///Assets/Tutorial_ThirdScreen_Icon.png"),
		};

		public static implicit operator OnboardingPageViewModel(OnboardingItemViewModel v)
		{
			throw new NotImplementedException();
		}
	}
}
