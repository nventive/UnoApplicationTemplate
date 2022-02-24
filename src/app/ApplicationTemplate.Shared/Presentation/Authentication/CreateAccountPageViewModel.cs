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

		public string[] Donuts =>
		new[]
		{
			"Cinnamon Twist",
			"Old-fashioned",
			"Jelly",
			"Double Glaze",
			"Cream Filled",
			"Apple Fritter"
		};

		public IDynamicCommand CreateAccount => this.GetCommandFromTask(async ct =>
		{
			var validationResult = await Form.Validate(ct);

			if (validationResult.IsValid)
			{
				await this.GetService<IAuthenticationService>().CreateAccount(ct, Form.Email.Trim(), Form.Password);

				await this.GetService<IStackNavigator>().NavigateAndClear(ct, () => new DadJokesPageViewModel());
			}
		});
	}
}
