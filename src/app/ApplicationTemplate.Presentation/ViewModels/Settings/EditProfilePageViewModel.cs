using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation;

public class EditProfilePageViewModel : ViewModel
{
	private readonly UserProfile _userProfile;

	public EditProfilePageViewModel(UserProfile userProfile)
	{
		_userProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));
	}

	public EditProfileFormViewModel Form => this.GetChild(() => new EditProfileFormViewModel(_userProfile));

	public IDynamicCommand UpdateProfile => this.GetCommandFromTask(async ct =>
	{
		var validationResult = await Form.Validate(ct);

		if (validationResult.IsValid)
		{
			var updatedUserProfile = _userProfile with
			{
				FirstName = Form.FirstName,
				LastName = Form.LastName
			};

			await this.GetService<IUserProfileService>().Update(ct, updatedUserProfile);

			await this.GetService<ISectionsNavigator>().NavigateBackOrCloseModal(ct);
		}
	});
}
