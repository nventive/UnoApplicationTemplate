using System.Threading.Tasks;

namespace ApplicationTemplate.Tests;

public static class FunctionalTestBaseExtensions
{
	public static async Task<LoginPageViewModel> ReachLoginPage(this FunctionalTestBase test)
	{
		await test.GetAndAssertActiveViewModel<OnboardingPageViewModel>().NavigateToNextPage.Execute();
		var vm = test.GetAndAssertActiveViewModel<LoginPageViewModel>();
		return vm;
	}

	public static async Task Login(this FunctionalTestBase test, LoginPageViewModel vm)
	{
		vm.Form.Email = "a@a";
		vm.Form.Password = "a";
		await vm.Login.Execute();
	}
}
