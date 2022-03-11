﻿using System;
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

		public bool PasswordIsEntered
		{
			get => this.Get<bool>(initialValue: false);
			set => this.Set(value);
		}

		public IDynamicCommand CreateAccount => this.GetCommandFromTask(async ct =>
		{
			var validationResult = await Form.Validate(ct);

			if (validationResult.IsValid && Form.PasswordHasEightCharacters && Form.PasswordHasNumber && Form.PasswordHasUppercase)
			{
				await this.GetService<IAuthenticationService>().CreateAccount(ct, Form.Email.Trim(), Form.Password);

				await this.GetService<IStackNavigator>().NavigateAndClear(ct, () => new DadJokesPageViewModel());
			}
		});

		public IDynamicCommand OnPasswordFocus => this.GetCommandFromTask(async ct =>
		{
			PasswordIsEntered = true;
		});
	}
}
