using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
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

		public bool PasswordHasEightCharacters
		{
			get => this.GetFromObservable(ObservePasswordHasEightCharacters());
			set => this.Set(value);
		}

		public bool PasswordHasNumber
		{
			get => this.GetFromObservable(ObservePasswordHasNumber());
			set => this.Set(value);
		}

		public bool PasswordHasUppercase
		{
			get => this.GetFromObservable(ObservePasswordHasUppercase());
			set => this.Set(value);
		}

		public bool PasswordIsEmpty
		{
			get => this.GetFromObservable(ObservePasswordIsEmpty());
			set => this.Set(value);
		}

		private IObservable<bool> ObservePasswordHasEightCharacters()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password => password.Length >= 8);
		}

		private IObservable<bool> ObservePasswordHasNumber()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password => password.Any(char.IsDigit));
		}

		private IObservable<bool> ObservePasswordHasUppercase()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password => password.Any(char.IsUpper));
		}

		private IObservable<bool> ObservePasswordIsEmpty()
		{
			var temp = Password.IsNullOrEmpty();
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password => password.IsNullOrEmpty());
		}
	}

	public class CreateAccountFormValidator : AbstractValidator<CreateAccountFormViewModel>
	{
		public CreateAccountFormValidator(IStringLocalizer localizer)
		{
			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Email"])
				.EmailAddress()
				.WithMessage(_ => localizer["ValidationError_Email"]);
		}
	}
}
