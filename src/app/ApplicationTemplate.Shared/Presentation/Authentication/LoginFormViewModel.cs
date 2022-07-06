using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate.Presentation
{
	public class LoginFormViewModel : ViewModel
	{
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

		public IDynamicCommand ValidateProperty => this.GetCommandFromTask<string>(async (ct, propertyName) =>
		{
			await this.ValidateProperty(ct, this.GetProperty(propertyName));
		});
	}

	public class LoginFormValidator : AbstractValidator<LoginFormViewModel>
	{
		public LoginFormValidator(IStringLocalizer localizer)
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Email"])
				.EmailAddress()
				.WithMessage(_ => localizer["ValidationError_Email"]);
			RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Password"]);
		}
	}
}
