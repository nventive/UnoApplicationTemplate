using System;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;
using MallardMessageHandlers;

namespace ApplicationTemplate.Presentation
{
	public partial class HomePageViewModel : ViewModel
	{
		public IDynamicCommand NavigateToJokes => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IStackNavigator>().Navigate(ct, () => new DadJokesPageViewModel());
		});

		public IDynamicCommand NavigateToOnboarding => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IStackNavigator>().Navigate(ct, () => new OnboardingPageViewModel());
		});
	}
}
