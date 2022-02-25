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
	/// <summary>
	/// Gives access to navigation methods.
	/// </summary>
	public class NavigationTestsBase : IntegrationTestBase
	{
		protected TViewModel GetAndAssertCurrentViewModel<TViewModel>()
		{
			var viewModel = GetCurrentViewModel();

			return Assert.IsType<TViewModel>(viewModel);
		}

		protected IViewModel GetCurrentViewModel()
		{
			return (IViewModel) GetService<ISectionsNavigator>().GetActiveViewModel();
		}

		protected async Task<ViewModel> NavigateAndClear(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			return await GetCurrentViewModel().GetService<ISectionsNavigator>().NavigateAndClear(ct, vmBuilder);
		}

		protected async Task<ViewModel> Navigate(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			return await GetCurrentViewModel().GetService<ISectionsNavigator>().Navigate(ct, vmBuilder);
		}

		protected async Task NavigateBack(CancellationToken ct)
		{
			await GetCurrentViewModel().GetService<ISectionsNavigator>().NavigateBackOrCloseModal(ct);
		}

		/// <summary>
		/// This method can be helpful when a test depends on navigation. (e.g. picker).
		/// </summary>
		/// <typeparam name="TViewModel">The target ViewModel.</typeparam>
		/// <param name="actionOnNavigation">The action to execute after navigating to the target ViewModel.</param>
		/// <returns><see cref="IDisposable"/>The subscription of the <see cref="ISectionsNavigator"/> observer.</returns>
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

		protected async Task<TDestinationViewModel> AssertNavigateFromTo<TSourceViewModel, TDestinationViewModel>(Func<TSourceViewModel> sourceVMBuilder, Func<TSourceViewModel, IDynamicCommand> navigationCommand)
			where TSourceViewModel : ViewModel
		{
			// Arrange
			TSourceViewModel viewModel = (TSourceViewModel) await NavigateAndClear(DefaultCancellationToken, sourceVMBuilder);

			// Act
			await navigationCommand(viewModel).Execute();

			// Assert
			return GetAndAssertCurrentViewModel<TDestinationViewModel>();
		}

		protected async Task<TDestinationViewModel> AssertNavigateFromToAfter<TSourceViewModel, TDestinationViewModel>(Func<IDynamicCommand> arrange, Func<TSourceViewModel, IDynamicCommand> act)
			where TSourceViewModel : ViewModel
		{
			// Arrange
			await arrange().Execute();

			// Act
			var viewModel = GetAndAssertCurrentViewModel<TSourceViewModel>();

			await act(viewModel).Execute();

			// Assert
			return GetAndAssertCurrentViewModel<TDestinationViewModel>();
		}

		protected async Task<TDestinationViewModel> AssertNavigateTo<TDestinationViewModel>(Func<IDynamicCommand> act)
		{
			// Act
			await act().Execute();

			// Assert
			return GetAndAssertCurrentViewModel<TDestinationViewModel>();
		}
	}
}
