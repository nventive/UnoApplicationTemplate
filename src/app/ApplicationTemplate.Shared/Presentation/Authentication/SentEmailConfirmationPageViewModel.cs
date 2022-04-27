using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation
{
	public class SentEmailConfirmationPageViewModel : ViewModel
	{
		public IDynamicCommand NavigateBackToLogin => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<ISectionsNavigator>().RemovePrevious(ct);
			await this.GetService<ISectionsNavigator>().NavigateBack(ct);
		});
	}
}
