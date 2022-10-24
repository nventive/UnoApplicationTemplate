using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests.Business;
public partial class UserProfileServiceShould
{
	[Fact]
	public async Task GetCurrentProfile()
	{
		// Arrange
		var mockedUserProfileEndpoint = new Mock<IUserProfileEndpoint>();
		mockedUserProfileEndpoint
			.Setup(endpoint => endpoint.Get(It.IsAny<CancellationToken>()))
			.ReturnsAsync(GetMockedUserProfile);

		var sut = new UserProfileService(mockedUserProfileEndpoint.Object);

		// Act
		var results = await sut.GetCurrent(CancellationToken.None);

		// Assert
		Assert.NotNull(results);
		Assert.IsType<UserProfile>(results);
	}

	[Fact]
	public async Task UpdateProfile_GivenAValidUserProfile()
	{
		var userProfile =
			new UserProfile { Id = "12345", FirstName = "Nventive", LastName = "Nventive", Email = "nventive@nventive.ca" };

		// Arrange
		var mockedUserProfileEndpoint = new Mock<IUserProfileEndpoint>();
		mockedUserProfileEndpoint
			.Setup(endpoint => endpoint.Get(It.IsAny<CancellationToken>()))
			.ReturnsAsync(userProfile.ToData());

		var sut = new UserProfileService(mockedUserProfileEndpoint.Object);

		var old = await sut.GetCurrent(CancellationToken.None);

		userProfile =
			new UserProfile { Id = "12345", FirstName = "Updated", LastName = "Nventive", Email = "nventive@nventive.ca" };

		mockedUserProfileEndpoint
			.Setup(endpoint => endpoint.Update(It.IsAny<CancellationToken>(), userProfile.ToData()));

		// Act
		await sut.Update(CancellationToken.None, userProfile);

		// Assert
		// TODO
	}
}
