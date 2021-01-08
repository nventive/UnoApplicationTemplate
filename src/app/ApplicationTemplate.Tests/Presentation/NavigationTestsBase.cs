using System;
using System.Collections.Generic;
using System.Linq;
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
		/// <summary>
		/// Navigates from the <typeparamref name="TSourceViewModel"/> to the <typeparamref name="TSourceViewModel"/>
		/// by executing the specified <paramref name="navigationCommand"/>.
		/// </summary>
		/// <typeparam name="TSourceViewModel">Type of the source ViewModel</typeparam>
		/// <typeparam name="TDestinationViewModel">Type of the destination ViewModel</typeparam>
		/// <param name="navigationCommand">Navigating command to </param>
		/// <returns>Destination ViewModel</returns>
		protected async Task<TDestinationViewModel> ExecuteNavigation<TSourceViewModel, TDestinationViewModel>(Func<TSourceViewModel, IDynamicCommand> navigationCommand)
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

		protected IViewModel GetCurrentViewModel()
		{
			return GetService<IStackNavigator>().State.Stack.LastOrDefault()?.ViewModel as IViewModel;
		}

		protected async Task StartNavigation(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			await GetCurrentViewModel().GetService<ISectionsNavigator>().NavigateAndClear(ct, vmBuilder);
		}

		protected async Task ForceNavigation(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			await GetCurrentViewModel().GetService<ISectionsNavigator>().Navigate(ct, vmBuilder);
		}

		protected IDisposable SubscribeToNavigation<TViewModel>(Action<CancellationToken> actionOnNavigation)
			where TViewModel : IViewModel
		{
			var navigatorService = GetService<ISectionsNavigator>();

			return navigatorService
				.ObserveActiveSectionLastPageType()
				.SelectManyDisposePrevious(async (type, ct) =>
				{
					if (type == typeof(TViewModel))
					{
						actionOnNavigation(ct);
					}
				})
				.Subscribe();
		}
	}
}
