using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Chinook.DynamicMvvm;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Presentation;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation
{
	public class CreateAccountFormViewModel : ViewModel
	{
		public CreateAccountFormViewModel()
		{
			this.AddValidation(this.GetProperty(x => x.Email));
			this.AddValidation(this.GetProperty(x => x.Password));
		}

		public string Email
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public string Password
		{
			get => this.Get<string>();
			set => this.Set(value);
		}
	}

	public class CreateAccountFormValidator : AbstractValidator<CreateAccountFormViewModel>
	{
		public CreateAccountFormValidator(IStringLocalizer localizer)
		{
			var myPassword = string.Empty;

			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress();

			RuleFor(x => x.Password)
				.NotEmpty()
				.Must(password =>
				{
					if (password == null)
					{
						return false;
					}

					myPassword = password;

					var longerThan8 = password?.Length >= 8;
					var containsNumber = password.Any(char.IsDigit);

					return longerThan8 && containsNumber;
				})
				.WithMessage(localizer["CreateAccount_PasswordValidation"]);
		}
	}
}
