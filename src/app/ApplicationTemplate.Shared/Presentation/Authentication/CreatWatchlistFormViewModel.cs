using System.Collections.Generic;
using System.Linq;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Uno;

namespace ApplicationTemplate.Presentation
{
	public class CreateWatchlistFormViewModel : ViewModel
	{
		public CreateWatchlistFormViewModel()
		{
			//WatchlistNames = watchlistNames;
			this.AddValidation(this.GetProperty(x => x.WatchlistName));
		}

		public string WatchlistName
		{
			get => this.Get<string>();
			set => this.Set(value);
		}

		public IEnumerable<string> WatchlistNames
		{
			get => this.Get<IEnumerable<string>>(initialValue: new string[] { "test" });
			set => this.Set(value);
		}
	}

	public class CreateWatchlistFormValidator : AbstractValidator<CreateWatchlistFormViewModel>
	{
		public CreateWatchlistFormValidator(IStringLocalizer localizer)
		{
			RuleFor(vm => vm).Must(x => x.WatchlistName != "test")
				.WithMessage(localizer["CreateWatchlist_ErrorDupplicate"]);
		}
	}
}
