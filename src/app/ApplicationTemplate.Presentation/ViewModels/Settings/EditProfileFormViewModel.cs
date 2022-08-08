using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using FluentValidation;

namespace ApplicationTemplate.Presentation;

public class EditProfileFormViewModel : ViewModel
{
	private readonly UserProfile _userProfileData;

	public EditProfileFormViewModel(UserProfile userProfile)
	{
		_userProfileData = userProfile ?? throw new ArgumentNullException(nameof(userProfile));

		this.AddValidation(this.GetProperty(x => x.FirstName));
		this.AddValidation(this.GetProperty(x => x.LastName));
	}

	public string FirstName
	{
		get => this.Get(_userProfileData.FirstName);
		set => this.Set(value);
	}

	public string LastName
	{
		get => this.Get(_userProfileData.LastName);
		set => this.Set(value);
	}
}

public class EditProfileFormValidator : AbstractValidator<EditProfileFormViewModel>
{
	public EditProfileFormValidator()
	{
		RuleFor(x => x.FirstName).NotEmpty();
		RuleFor(x => x.LastName).NotEmpty();
	}
}
