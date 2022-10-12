using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationTemplate.Client;
using ApplicationTemplate.Presentation;

namespace ApplicationTemplate.Tests;

public partial class NavigationShould
{
	public CreateAccountFormViewModel _mockedAccount = new CreateAccountFormViewModel();
	public PasswordFormViewModel _mockedPassword = new PasswordFormViewModel();

	public CreateAccountFormViewModel GetMockedAccount()
	{
		_mockedAccount.FirstName = "Nventive";
		_mockedAccount.LastName = "Nventive";
		_mockedAccount.Email = "nventive@nventive.com";
		_mockedAccount.PhoneNumber = "(111) 111 1111";
		_mockedAccount.PostalCode = "A1A 1A1";
		_mockedAccount.DateOfBirth = DateTimeOffset.Parse("7/6/1994");
		_mockedAccount.FavoriteDadNames = new[]
		{
			"Dad",
			"Papa",
			"Pa",
			"Pop",
			"Father",
			"Padre",
			"Père",
			"Papi",
		};
		_mockedAccount.AgreeToTermsOfServices = true;

		return _mockedAccount;
	}

	public PasswordFormViewModel GetMockedPassword()
	{
		_mockedPassword.Password = "Abcdef12";

		return _mockedPassword;
	}
}
