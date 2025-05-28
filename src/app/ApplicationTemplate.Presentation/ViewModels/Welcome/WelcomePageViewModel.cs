using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation;

public sealed class WelcomePageViewModel : ViewModel
{
	public IDynamicCommand NavigateToOnboarding => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new OnboardingPageViewModel());
	});
}
