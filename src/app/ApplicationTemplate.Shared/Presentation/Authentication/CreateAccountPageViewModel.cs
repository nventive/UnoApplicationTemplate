using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation
{
	public class CreateAccountPageViewModel : ViewModel
	{
		public CreateAccountFormViewModel Form => this.GetChild(() => new CreateAccountFormViewModel());

		public PasswordFormViewModel PasswordForm => this.GetChild(() => new PasswordFormViewModel());

		public string[] DadNames =>
		new[]
		{
			"Dad",
			"Papa",
			"Pa",
			"Pop",
			"Father",
			"Padre",
			"Père",
			"Papi",
		};

		public IDynamicCommand CreateAccount => this.GetCommandFromTask(async ct =>
		{
			var validationResult = await Form.Validate(ct);

			if (validationResult.IsValid && PasswordForm.PasswordHasMinimumLength == true && PasswordForm.PasswordHasNumber == true && PasswordForm.PasswordHasUppercase == true)
			{
				await this.GetService<IAuthenticationService>().CreateAccount(ct, Form.Email.Trim(), PasswordForm.Password);

				await this.GetService<ISectionsNavigator>().SetActiveSection(ct, "Home", () => new DadJokesPageViewModel());
			}
		});
	}
}
