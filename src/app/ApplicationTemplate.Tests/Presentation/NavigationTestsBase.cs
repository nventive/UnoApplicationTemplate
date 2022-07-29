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
		/// <summary>
		/// This method can be used to assert that the current ViewModel is a TViewModel.
		/// </summary>
		/// <typeparam name="TViewModel">The expected ViewModel type.</typeparam>
		/// <returns>The instance of TViewModel.</returns>
		protected TViewModel GetAndAssertCurrentViewModel<TViewModel>()
		{
			var viewModel = GetCurrentViewModel();

			return Assert.IsType<TViewModel>(viewModel);
		}

		/// <summary>
		/// This method can be used to get the current ViewModel.
		/// </summary>
		/// <returns>The current ViewModel.</returns>
		protected IViewModel GetCurrentViewModel()
		{
			return (IViewModel)GetService<ISectionsNavigator>().GetActiveStackNavigator().GetActiveViewModel();
		}

		/// <summary>
		/// The method can be used to navigate to a new page and clear navigation stack.
		/// </summary>
		/// <param name="ct">The cancellation token.</param>
		/// <param name="vmBuilder">The ViewModel builder.</param>
		/// <returns>A task containing the destination view model.</returns>
		protected async Task<ViewModel> NavigateAndClear(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			return await GetService<ISectionsNavigator>().NavigateAndClear(ct, vmBuilder);
		}

		/// <summary>
		/// The method can be used to navigate to a new page.
		/// </summary>
		/// <param name="ct">The cancellation token.</param>
		/// <param name="vmBuilder">The ViewModel builder.</param>
		/// <returns>A task containing the destination view model.</returns>
		protected async Task<ViewModel> Navigate(CancellationToken ct, Func<ViewModel> vmBuilder)
		{
			return await GetCurrentViewModel().GetService<ISectionsNavigator>().Navigate(ct, vmBuilder);
		}

		/// <summary>
		/// The method can be used to set an active section and navigate to it.
		/// </summary>
		/// <param name="ct">The cancellation token.</param>
		/// <param name="sectionName">A string of the next section name.</param>
		/// <param name="vmBuilder">The ViewModel builder.</param>
		protected async Task SetActiveSection(CancellationToken ct, string sectionName, Func<ViewModel> vmBuilder)
		{
			await GetService<ISectionsNavigator>().SetActiveSection(ct, sectionName, vmBuilder);
		}

		/// <summary>
		/// This method can be used to go to the previous page or modal.
		/// </summary>
		/// <param name="ct">The cancellation token.</param>
		/// <returns>A task that when completed will go to the previous page or modal.</returns>
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

		/// <summary>
		/// This method can be used to assert if a command provides the given destination.
		/// </summary>
		/// <typeparam name="TSourceViewModel">The source ViewModel type.</typeparam>
		/// <typeparam name="TDestinationViewModel">The destination ViewModel type.</typeparam>
		/// <param name="sourceVMBuilder">The source ViewModel builder.</param>
		/// <param name="navigationCommand">A function that returns a command to execute before asserting the destination is TDestinationViewModel</param>
		/// <returns>A task that when completed will contain the destination ViewModel.</returns>
		protected async Task<TDestinationViewModel> AssertNavigateFromTo<TSourceViewModel, TDestinationViewModel>(Func<TSourceViewModel> sourceVMBuilder, Func<TSourceViewModel, IDynamicCommand> navigationCommand)
			where TSourceViewModel : ViewModel
		{
			// Arrange
			TSourceViewModel viewModel = (TSourceViewModel)await NavigateAndClear(DefaultCancellationToken, sourceVMBuilder);

			// Act
			await navigationCommand(viewModel).Execute();

			// Assert
			return GetAndAssertCurrentViewModel<TDestinationViewModel>();
		}

		/// <summary>
		/// This method can be used to assert if a command changes the section.
		/// </summary>
		/// <typeparam name="TSourceViewModel">The source ViewModel type.</typeparam>
		/// <typeparam name="TDestinationViewModel">The destination ViewModel type.</typeparam>
		/// <param name="sourceVMBuilder">The source ViewModel builder.</param>
		/// <param name="navigationCommand">A function that returns a command to execute before asserting the destination is TDestinationViewModel</param>
		/// <param name="sourceSection">The section name.</param>
		/// <returns>A task that when completed will contain the new section name.</returns>
		protected async Task<string> AssertSetActiceSection<TSourceViewModel, TDestinationViewModel>(Func<TSourceViewModel> sourceVMBuilder, Func<TSourceViewModel, IDynamicCommand> navigationCommand, string sourceSection)
			where TSourceViewModel : ViewModel
		{
			await SetActiveSection(DefaultCancellationToken, sourceSection, sourceVMBuilder);
			TSourceViewModel viewModel = (TSourceViewModel)await NavigateAndClear(DefaultCancellationToken, sourceVMBuilder);

			await navigationCommand(viewModel).Execute();

			return GetService<ISectionsNavigator>().State.ActiveSection.Name;
		}

		/// <summary>
		/// This method will assert that the TDestinationViewModel is the current ViewModel after executing the IDynamicCommand.
		/// </summary>
		/// <typeparam name="TDestinationViewModel">The expected ViewModel type</typeparam>
		/// <param name="act">The IDynamicCommand provider.</param>
		/// <returns>A task that when completed will contain the expected ViewModel.</returns>
		protected async Task<TDestinationViewModel> AssertNavigateTo<TDestinationViewModel>(Func<IDynamicCommand> act)
		{
			// Act
			await act().Execute();

			// Assert
			return GetAndAssertCurrentViewModel<TDestinationViewModel>();
		}
	}
}
