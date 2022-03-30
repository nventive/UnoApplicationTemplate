using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Chinook.DynamicMvvm;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation
{
	public class ResetPasswordPageViewModel : ViewModel
	{
		public string Password
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public PasswordState PasswordHasEightCharacters
		{
			get => this.GetFromObservable(ObservePasswordHasEightCharacters(), initialValue: PasswordState.Unedited);
			set => this.Set(value);
		}

		public PasswordState PasswordHasNumber
		{
			get => this.GetFromObservable(ObservePasswordHasNumber(), initialValue: PasswordState.Unedited);
			set => this.Set(value);
		}

		public PasswordState PasswordHasUppercase
		{
			get => this.GetFromObservable(ObservePasswordHasUppercase(), initialValue: PasswordState.Unedited);
			set => this.Set(value);
		}

		private IObservable<PasswordState> ObservePasswordHasEightCharacters()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return PasswordState.Unedited;
					}

					return password.Length >= 8 ? PasswordState.Valid : PasswordState.Invalid;
				});
		}

		private IObservable<PasswordState> ObservePasswordHasNumber()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return PasswordState.Unedited;
					}

					return password.Any(char.IsDigit) ? PasswordState.Valid : PasswordState.Invalid;
				});
		}

		private IObservable<PasswordState> ObservePasswordHasUppercase()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return PasswordState.Unedited;
					}

					return password.Any(char.IsUpper) ? PasswordState.Valid : PasswordState.Invalid;
				});
		}
	}
}
