using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation;

public class ForgotPasswordPageViewModel : ViewModel
{
	public ForgotPasswordFormViewModel Form => this.GetChild(() => new ForgotPasswordFormViewModel());

	public IDynamicCommand SendLink => this.GetCommandFromTask(async ct =>
	{
		var validationResult = await Form.Validate(ct);

		// Send link here - https://dev.azure.com/nventive/Practice%20committees/_backlogs/backlog/Mobile%20(.Net)/Features/?workitem=245437

		if (validationResult.IsValid)
		{
			await this.GetService<ISectionsNavigator>().Navigate(ct, () => new SentEmailConfirmationPageViewModel());
			await this.GetService<ISectionsNavigator>().RemovePrevious(ct);
		}
	});

	public IDynamicCommand ResetPassword => this.GetCommandFromTask(async ct =>
	{
		var validationResult = await Form.Validate(ct);

		if (validationResult.IsValid)
		{
			await this.GetService<IAuthenticationService>().ResetPassword(ct, Form.Email.Trim());

			await this.GetService<ISectionsNavigator>().SetActiveSection(ct, "Home", () => new DadJokesPageViewModel());
		}
	});
}
