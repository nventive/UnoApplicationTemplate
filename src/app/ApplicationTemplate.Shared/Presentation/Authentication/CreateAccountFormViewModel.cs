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
			this.AddValidation(this.GetProperty(x => x.FirstName));
			this.AddValidation(this.GetProperty(x => x.LastName));
			this.AddValidation(this.GetProperty(x => x.Email));
			this.AddValidation(this.GetProperty(x => x.PhoneNumber));
			this.AddValidation(this.GetProperty(x => x.PostalCode));
			this.AddValidation(this.GetProperty(x => x.DateOfBirth));
			this.AddValidation(this.GetProperty(x => x.AgreeToTermsOfServices));
		}

		public string FirstName
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public string LastName
		{
			get => this.Get<string>();
			set => this.Set(value);
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

		public string PhoneNumber
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public string PostalCode
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public DateTimeOffset DateOfBirth
		{
			get => this.Get<DateTimeOffset>(initialValue: DateTimeOffset.Now);
			set => this.Set(value);
		}

		public object[] FavoriteDadNames
		{
			get => this.Get(initialValue: Array.Empty<object>());
			set => this.Set(value);
		}

		public bool AgreeToTermsOfServices
		{
			get => this.Get<bool>();
			set => this.Set(value);
		}

		public bool? PasswordHasEightCharacters
		{
			get => this.GetFromObservable(ObservePasswordHasEightCharacters(), initialValue: null);
			set => this.Set(value);
		}

		public bool? PasswordHasNumber
		{
			get => this.GetFromObservable(ObservePasswordHasNumber(), initialValue: null);
			set => this.Set(value);
		}

		public bool? PasswordHasUppercase
		{
			get => this.GetFromObservable(ObservePasswordHasUppercase(), initialValue: null);
			set => this.Set(value);
		}

		private IObservable<bool?> ObservePasswordHasEightCharacters()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select<string, bool?>(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return null;
					}

					return password.Length >= 8;
				});
		}

		private IObservable<bool?> ObservePasswordHasNumber()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select<string, bool?>(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return null;
					}

					return password.Any(char.IsDigit);
				});
		}

		private IObservable<bool?> ObservePasswordHasUppercase()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select<string, bool?>(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return null;
					}

					return password.Any(char.IsUpper);
				});
		}
	}

	public class CreateAccountFormValidator : AbstractValidator<CreateAccountFormViewModel>
	{
		public CreateAccountFormValidator(IStringLocalizer localizer)
		{
			RuleFor(x => x.FirstName).NotEmpty();
			RuleFor(x => x.LastName).NotEmpty();

			RuleFor(x => x.Email)
				.NotEmpty()
				.WithMessage(_ => localizer["ValidationNotEmpty_Email"])
				.EmailAddress()
				.WithMessage(_ => localizer["ValidationError_Email"]);

			RuleFor(x => x.PhoneNumber)
				.NotEmpty()
				.MustBePhoneNumber()
				.WithMessage(localizer["CreateAccount_PhoneNumberValidation"]);

			RuleFor(x => x.PostalCode)
				.NotEmpty()
				.Length(7)
				.WithMessage(localizer["CreateAccount_PostalValidation"]); // "A1A 1A1".Length = 7

			RuleFor(x => x.DateOfBirth)
				.NotEmpty()
				.MustBe18OrOlder()
				.WithMessage(localizer["CreateAccount_DateOfBirthValidation"]);

			RuleFor(x => x.FavoriteDadNames)
				.Must(favDadNames =>
				{
					if (favDadNames == null)
					{
						return false;
					}
					return favDadNames.Length >= 1;
				})
				.WithMessage(localizer["CreateAccount_FavoriteDadNameValidation"]);

			RuleFor(x => x.AgreeToTermsOfServices)
				.Equal(true)
				.WithMessage(localizer["CreateAccount_TermsOfServiceValidation"]);

		}
	}
}
