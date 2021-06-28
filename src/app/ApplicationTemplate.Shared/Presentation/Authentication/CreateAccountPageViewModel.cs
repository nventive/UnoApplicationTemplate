using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation
{
	public class CreateAccountPageViewModel : ViewModel
	{
		public CreateAccountFormViewModel Form => this.GetChild(() => new CreateAccountFormViewModel());

		public CreateWatchlistFormViewModel Form2 => this.GetChild(() => new CreateWatchlistFormViewModel());

		public IDynamicCommand CreateAccount => this.GetCommandFromTask(async ct =>
		{
			var validationResult = await Form.Validate(ct);	
			var validationResult2 = await Form2.Validate(ct);

			if (validationResult.IsValid)
			{
				await this.GetService<IAuthenticationService>().CreateAccount(ct, Form.Email.Trim(), Form.Password);

				await this.GetService<IStackNavigator>().NavigateAndClear(ct, () => new HomePageViewModel());
			}
		});
	}
}
