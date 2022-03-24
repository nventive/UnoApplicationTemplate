using System;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace ApplicationTemplate.Presentation
{
	public class LoginPageViewModel : ViewModel
	{
		private readonly Func<CancellationToken, Task> _onSuccessfulLogin;
		public LoginPageViewModel(Func<CancellationToken, Task> onSuccessfulLogin, bool isFirstLogin)
		{
			_onSuccessfulLogin = onSuccessfulLogin ?? throw new ArgumentNullException(nameof(onSuccessfulLogin));
			this.Title = isFirstLogin ? this.GetService<IStringLocalizer>()["Login_Title1"] : this.GetService<IStringLocalizer>()["Login_Title2"];
			this.Quote = isFirstLogin ? this.GetService<IStringLocalizer>()["Login_Subtitle1"] : this.GetService<IStringLocalizer>()["Login_Subtitle2"];
		}
		public LoginFormViewModel Form => this.GetChild(() => new LoginFormViewModel());
		public string Title
		{
			get => this.Get<string>();
			set => this.Set(value);
		}
		public string Quote
		{
			get => this.Get<string>();
			set => this.Set(value);
		}
		public IDynamicCommand Login => this.GetCommandFromTask(async ct =>
		{
			var validationResult = await Form.Validate(ct);

			if (validationResult.IsValid)
			{
				await this.GetService<IAuthenticationService>().Login(ct, Form.Email.Trim(), Form.Password);

				await _onSuccessfulLogin.Invoke(ct);
			}
		});
		public IDynamicCommand NavigateToCreateAccountPage => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IStackNavigator>().Navigate(ct, () => new CreateAccountPageViewModel());
		});
		public IDynamicCommand NavigateToForgotPasswordPage => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<IStackNavigator>().Navigate(ct, () => new ForgotPasswordPageViewModel());
		});
	}
}
