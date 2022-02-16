using System;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation
{
	public partial class OnboardingPageViewModel : ViewModel
	{
		public IDynamicCommand NavigateToJokes => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IApplicationSettingsService>().CompleteOnboarding(ct);

			await this.GetService<ISectionsNavigator>().NavigateAndClear(ct, () => new DadJokesPageViewModel());
		});

		public OnboardingItemViewModel[] OnboardingItems { get; } = new[]
		{
			new OnboardingItemViewModel("Donec faucibus erat vitae dui mattis lacinia. Nullam varius ultricies libero, sit amet tristique ex maximus ut. Donec erat felis, tempus sodales dolor ut, consequat maximus lacus.", "ms-appx:///Assets/Placeholder.png"),
			new OnboardingItemViewModel("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean eleifend sem eu orci pulvinar rutrum. Morbi quis nisi ac augue venenatis faucibus.", "ms-appx:///Assets/Placeholder.png"),
			new OnboardingItemViewModel("Pellentesque a pharetra libero. Nunc rhoncus leo auctor ipsum placerat pharetra venenatis placerat eros. Aenean tincidunt consequat tincidunt. Pellentesque eu orci dapibus dui consequat elementum ultrices non quam. Nulla et fermentum dui. Fusce aliquam nec erat quis tincidunt.", "ms-appx:///Assets/Placeholder.png"),
		};

		public static implicit operator OnboardingPageViewModel(OnboardingItemViewModel v)
		{
			throw new NotImplementedException();
		}
	}
}
