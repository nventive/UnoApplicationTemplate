using System;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate.Tests;

public partial class NavigationShould
{
	/// <summary>
	/// Navigates from the <typeparamref name="TSourceViewModel"/> to the <typeparamref name="TDestinationViewModel"/>
	/// by executing the specified <paramref name="navigationCommand"/>.
	/// </summary>
	/// <typeparam name="TSourceViewModel">Type of the source ViewModel</typeparam>
	/// <typeparam name="TDestinationViewModel">Type of the destination ViewModel</typeparam>
	/// <param name="navigationCommand">Navigationg command to </param>
	/// <returns>Destination ViewModel</returns>
	private async Task<TDestinationViewModel> AssertNavigateFromTo<TSourceViewModel, TDestinationViewModel>(Func<TSourceViewModel, IDynamicCommand> navigationCommand)
		where TSourceViewModel : IViewModel
	{
		var viewModel = GetAndAssertCurrentViewModel<TSourceViewModel>();

		await navigationCommand(viewModel).Execute();

		return GetAndAssertCurrentViewModel<TDestinationViewModel>();
	}
}
