﻿using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.Localization;
using Presentation;

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
			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Email"])
				.IsValidEmail()
				.WithMessage(_ => localizer["ValidationError_Email"]);
			RuleFor(x => x.Password)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Password"]);
		}
	}
}
