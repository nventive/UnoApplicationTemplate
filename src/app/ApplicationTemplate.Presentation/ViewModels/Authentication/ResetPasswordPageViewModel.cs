using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation
{
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
}
