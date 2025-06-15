using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation;

public class ResetPasswordPageViewModel : ViewModel
{
	public PasswordFormViewModel PasswordForm => this.GetChild(() => new PasswordFormViewModel());

	public IDynamicCommand ConfirmReset => this.GetCommandFromTask(async ct =>
	{
		if (PasswordForm.PasswordHasMinimumLength == true && PasswordForm.PasswordHasNumber == true && PasswordForm.PasswordHasUppercase == true)
		{
			await this.GetService<ISectionsNavigator>().NavigateBack(ct);
		}
		else
		{
			PasswordForm.ValidatePasswordHints();
		}
	});
}
