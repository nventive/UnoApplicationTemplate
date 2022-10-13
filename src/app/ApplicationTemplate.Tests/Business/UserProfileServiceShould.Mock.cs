using ApplicationTemplate.Client;

namespace ApplicationTemplate.Tests.Business;

public partial class UserProfileServiceShould
{
	private UserProfileData _mockedUserProfileData =
		new UserProfileData("12345", "Nventive", "Nventive", "nventive@nventive.ca");

	private UserProfileData GetMockedUserProfile()
	{
		return _mockedUserProfileData;
	}
}
