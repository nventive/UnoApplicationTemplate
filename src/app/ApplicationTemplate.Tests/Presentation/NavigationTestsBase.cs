using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class NavigationTestsBase : TestBase
	{
		protected TViewModel GetAndAssertCurrentViewModel<TViewModel>()
		{
			var viewModel = GetCurrentViewModel();

			return Assert.IsType<TViewModel>(viewModel);
		}

		protected IViewModel GetCurrentViewModel()
		{
			return GetService<IStackNavigator>().State.Stack.LastOrDefault()?.ViewModel as IViewModel;
		}

		protected async Task NavigateAndClear(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			await GetCurrentViewModel().GetService<ISectionsNavigator>().NavigateAndClear(ct, vmBuilder);
		}

		protected async Task Navigate(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			await GetCurrentViewModel().GetService<ISectionsNavigator>().Navigate(ct, vmBuilder);
		}

		/// <summary>
		/// This method can be helpful when a test depends on navigation. (e.g. picker).
		/// </summary>
		/// <typeparam name="TViewModel">Target ViewModel.</typeparam>
		/// <param name="actionOnNavigation">Action to execute after navigating to the target ViewModel.</param>
		/// <returns><see cref="IDisposable"/>.</returns>
		protected IDisposable SubscribeToNavigation<TViewModel>(Action actionOnNavigation)
			where TViewModel : IViewModel
		{
			var navigatorService = GetService<ISectionsNavigator>();

			return navigatorService
				.ObserveActiveSectionLastPageType()
				.Select(type =>
				{
					if (type == typeof(TViewModel))
					{
						actionOnNavigation();
					}

					return Unit.Default;
				})
				.Subscribe();
		}
	}
}
