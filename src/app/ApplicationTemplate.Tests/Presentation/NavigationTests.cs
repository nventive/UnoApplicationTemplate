using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class NavigationTests : TestBase
	{
		[Fact]
		public async Task It_Should_Navigate_Everywhere()
		{
			await ExecuteNavigation<OnboardingPageViewModel, WelcomePageViewModel>(p => p.NavigateToWelcomePage);

			await ExecuteNavigation<WelcomePageViewModel, CreateAccountPageViewModel>(p => p.NavigateToCreateAccountPage);

			await ExecuteNavigation<CreateAccountPageViewModel, WelcomePageViewModel>(p => p.NavigateBack);

			await ExecuteNavigation<WelcomePageViewModel, LoginPageViewModel>(p => p.NavigateToLoginPage);

			await ExecuteNavigation<LoginPageViewModel, WelcomePageViewModel>(p => p.NavigateBack);

			await ExecuteNavigation<WelcomePageViewModel, HomePageViewModel>(p => p.NavigateToHomePage);

			await ExecuteNavigation<HomePageViewModel, PostsPageViewModel>(p => p.NavigateToPostsPage);

			await ExecuteNavigation<PostsPageViewModel, EditPostPageViewModel>(p => p.NavigateToNewPost);

			await ExecuteNavigation<EditPostPageViewModel, PostsPageViewModel>(p => p.NavigateBack);

			await ExecuteNavigation<PostsPageViewModel, EditPostPageViewModel>(p => p.NavigateToNewPost);

			await ExecuteNavigation<EditPostPageViewModel, PostsPageViewModel>(p => p.NavigateBack);

			await ExecuteNavigation<PostsPageViewModel, HomePageViewModel>(p => p.NavigateBack);

			await ExecuteNavigation<HomePageViewModel, SettingsPageViewModel>(p => p.NavigateToSettingsPage);

			await ExecuteNavigation<SettingsPageViewModel, DiagnosticsPageViewModel>(p => p.NavigateToDiagnosticsPage);

			await ExecuteNavigation<DiagnosticsPageViewModel, SettingsPageViewModel>(p => p.NavigateBack);

			await ExecuteNavigation<SettingsPageViewModel, HomePageViewModel>(p => p.NavigateBack);
		}

		/// <summary>
		/// Navigates from the <typeparamref name="TSourceViewModel"/> to the <typeparamref name="TSourceViewModel"/>
		/// by executing the specified <paramref name="navigationCommand"/>.
		/// </summary>
		/// <typeparam name="TSourceViewModel">Type of the source ViewModel</typeparam>
		/// <typeparam name="TDestinationViewModel">Type of the destination ViewModel</typeparam>
		/// <param name="navigationCommand">Navigationg command to </param>
		/// <returns>Destination ViewModel</returns>
		private async Task<TDestinationViewModel> ExecuteNavigation<TSourceViewModel, TDestinationViewModel>(Func<TSourceViewModel, IDynamicCommand> navigationCommand)
			where TSourceViewModel : IViewModel
		{
			var viewModel = GetAndAssertCurrentViewModel<TSourceViewModel>();

			await navigationCommand(viewModel).Execute();

			return GetAndAssertCurrentViewModel<TDestinationViewModel>();
		}

		private TViewModel GetAndAssertCurrentViewModel<TViewModel>()
		{
			var viewModel = GetCurrentViewModel();

			return Assert.IsType<TViewModel>(viewModel);
		}

		private IViewModel GetCurrentViewModel()
		{
			return GetService<IStackNavigator>().State.Stack.LastOrDefault()?.ViewModel as IViewModel;
		}
	}
}
