using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Localization;
using Presentation;

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
		public ForgotPasswordFormValidator(IStringLocalizer localizer)
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Email"])
				.IsValidEmail()
				.WithMessage(_ => localizer["ValidationError_Email"]);
		}
	}
}
