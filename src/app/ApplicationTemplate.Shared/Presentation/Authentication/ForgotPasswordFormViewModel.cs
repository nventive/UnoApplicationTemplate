using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using FluentValidation;
using FluentValidation.Validators;

namespace ApplicationTemplate.Presentation
{
	public class ForgotPasswordFormViewModel : ViewModel
	{
		public ForgotPasswordFormViewModel()
		{
			this.AddValidation(this.GetProperty(x => x.Email));
		}

		public string Email
		{
			get => this.Get<string>();
			set => this.Set(value);
		}
	}

	public class ForgotPasswordFormValidator : AbstractValidator<ForgotPasswordFormViewModel>
	{
		public ForgotPasswordFormValidator()
		{
#pragma warning disable CS0618 // EmailValidationMode.Net4xRegex validates for A@A.A. Default mode is only checking for A@A
			RuleFor(x => x.Email)
				.NotEmpty()
				.EmailAddress(EmailValidationMode.Net4xRegex);
#pragma warning restore CS0618 // Type or member is obsolete
		}
	}
}
