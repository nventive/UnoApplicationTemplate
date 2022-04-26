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
		public LoginFormViewModel()
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

	public class LoginFormValidator : AbstractValidator<LoginFormViewModel>
	{
		public LoginFormValidator(IStringLocalizer localizer)
		{
#pragma warning disable CS0618 // EmailValidationMode.Net4xRegex validates for A@A.A. Default mode is only checking for A@A
			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Email"])
				.EmailAddress(EmailValidationMode.Net4xRegex)
				.WithMessage(_ => localizer["ValidationError_Email"]);
#pragma warning restore CS0618 // Type or member is obsolete
			RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Password"]);
		}
	}
}
