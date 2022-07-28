using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Chinook.DynamicMvvm;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation
{
	public class PasswordFormViewModel : ViewModel
	{
		public string Password
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public bool? PasswordHasMinimumLength
		{
			get => this.GetFromObservable(ObservePasswordHasMinimumLength(), initialValue: null);
			private set => this.Set(value);
		}

		public bool? PasswordHasNumber
		{
			get => this.GetFromObservable(ObservePasswordHasNumber(), initialValue: null);
			private set => this.Set(value);
		}

		public bool? PasswordHasUppercase
		{
			get => this.GetFromObservable(ObservePasswordHasUppercase(), initialValue: null);
			private set => this.Set(value);
		}

		private IObservable<bool?> ObservePasswordHasMinimumLength()
		{
			return this.GetProperty(x => x.Password)
				.Observe()
				.Select<string, bool?>(password =>
				{
					if (password.IsNullOrEmpty())
					{
						return null;
					}

					return password.Length >= PresentationConstants.PasswordMinLength;
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

		public void ValidatePasswordHints()
		{
			if (Password.IsNullOrEmpty())
			{
				// A password was not entered, we need to trigger validation hints
				PasswordHasMinimumLength = false;
				PasswordHasNumber = false;
				PasswordHasUppercase = false;
			}
		}
	}
}
