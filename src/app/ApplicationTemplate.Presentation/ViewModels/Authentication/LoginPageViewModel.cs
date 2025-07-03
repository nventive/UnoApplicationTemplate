// src/app/ApplicationTemplate.Presentation/ViewModels/Authentication/LoginPageViewModel.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess.PlatformServices;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate.Presentation;

public class LoginPageViewModel : ViewModel
{
	public LoginPageViewModel(bool isFirstLogin)
	{
		IsFirstLogin = isFirstLogin;
	}

	public LoginFormViewModel Form => this.GetChild(() => new LoginFormViewModel());

	public string Title
	{
		get => IsFirstLogin ? this.GetService()["Login_Title1"] : this.GetService()["Login_Title2"];
		set => this.Set(value);
	}

	public string Quote
	{
		get => IsFirstLogin ? this.GetService()["Login_TitleMedium"] : this.GetService()["Login_TitleSmall"];
		set => this.Set(value);
	}

	public string Version
	{
		get => $"{this.GetService().VersionString} ({this.GetService().BuildString})";
		set => this.Set(value);
	}

	public bool IsFirstLogin
	{
		get => this.Get();
		set => this.Set(value);
	}

	public IDynamicCommand Login => this.GetCommandFromTask(async ct =>
	{
		var validationResult = await Form.Validate(ct);

		if (validationResult.IsValid)
		{
			await this.GetService().Login(ct, Form.Email.Trim(), Form.Password);
			await NavigateToHome.Execute();
		}
	});

	public IDynamicCommand NavigateToHome => this.GetCommandFromTask(async ct =>
	{
		await this.GetService().SetActiveSection(ct, "Home", () => new DadJokesPageViewModel());
	});

	public IDynamicCommand NavigateToCreateAccountPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService().Navigate(ct, () => new CreateAccountPageViewModel());
	});

	public IDynamicCommand NavigateToForgotPasswordPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService().Navigate(ct, () => new ForgotPasswordPageViewModel());
	});
}
