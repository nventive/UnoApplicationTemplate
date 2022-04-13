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
}
