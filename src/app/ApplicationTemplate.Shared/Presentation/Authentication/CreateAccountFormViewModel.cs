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

namespace ApplicationTemplate.Presentation;

public class CreateAccountFormViewModel : ViewModel
{
	public CreateAccountFormViewModel()
	{
		this.AddValidation(this.GetProperty(x => x.FirstName));
		this.AddValidation(this.GetProperty(x => x.LastName));
		this.AddValidation(this.GetProperty(x => x.Email));
		this.AddValidation(this.GetProperty(x => x.PhoneNumber));
		this.AddValidation(this.GetProperty(x => x.SecondaryPhoneNumber));
		this.AddValidation(this.GetProperty(x => x.PostalCode));
		this.AddValidation(this.GetProperty(x => x.DateOfBirth));
		this.AddValidation(this.GetProperty(x => x.Password));
		this.AddValidation(this.GetProperty(x => x.ConfirmPassword));
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

	public string PhoneNumber
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	public string SecondaryPhoneNumber
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

	public string Password
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	public string ConfirmPassword
	{
		get => this.Get<string>();
		set => this.Set(value);
	}

	public object[] FavoriteDonuts
	{
		get => this.Get(initialValue: Array.Empty<object>());
		set => this.Set(value);
	}

	public bool AgreeToTermsOfServices
	{
		get => this.Get<bool>();
		set => this.Set(value);
	}
}

public class CreateAccountFormValidator : AbstractValidator<CreateAccountFormViewModel>
{
	public CreateAccountFormValidator(IStringLocalizer localizer)
	{
		var myPassword = string.Empty;

		RuleFor(x => x.FirstName).NotEmpty();
		RuleFor(x => x.LastName).NotEmpty();

		RuleFor(x => x.Email)
			.NotEmpty()
			.EmailAddress();

		RuleFor(x => x.PhoneNumber)
			.NotEmpty()
			.MustBePhoneNumber()
			.WithMessage(localizer["CreateAccount_PhoneNumberValidation"]);

		RuleFor(x => x.SecondaryPhoneNumber)
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

		RuleFor(x => x.ConfirmPassword)
			.NotEmpty()
			.Must(confrimPasswor => confrimPasswor == myPassword)
			.WithMessage(localizer["CreateAccount_PasswordConfirmValidation"]);

		RuleFor(x => x.FavoriteDonuts)
			.Must(favDonuts =>
			{
				if (favDonuts == null)
				{
					return false;
				}
				return favDonuts.Length >= 3;
			})
			.WithMessage(localizer["CreateAccount_FavoriteDonutsValidation"]);

		RuleFor(x => x.AgreeToTermsOfServices)
			.Equal(true)
			.WithMessage(localizer["CreateAccount_TermsOfServiceValidation"]);
	}
}
