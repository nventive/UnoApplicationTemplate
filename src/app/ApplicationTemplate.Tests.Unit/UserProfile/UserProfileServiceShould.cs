using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace ApplicationTemplate.Tests;

public sealed partial class UserProfileServiceShould
{
	[Fact]
	public async Task GetCurrentProfile()
	{
		// Arrange
		var mockedUserProfileEndpoint = Substitute.For<IUserProfileEndpoint>();
		mockedUserProfileEndpoint
			.Get(Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(GetMockedUserProfile()));

		var sut = new UserProfileService(mockedUserProfileEndpoint);

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
		var mockedUserProfileEndpoint = Substitute.For<IUserProfileEndpoint>();
		mockedUserProfileEndpoint
			.Get(Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(userProfile.ToData()));

		var sut = new UserProfileService(mockedUserProfileEndpoint);

		var old = await sut.GetCurrent(CancellationToken.None);

		userProfile =
			new UserProfile { Id = "12345", FirstName = "Updated", LastName = "Nventive", Email = "nventive@nventive.ca" };

		mockedUserProfileEndpoint
			.Get(Arg.Any<CancellationToken>())
			.Returns(userProfile.ToData());

		// Act
		await sut.Update(CancellationToken.None, userProfile);
		var updated = await sut.GetCurrent(CancellationToken.None);

		// Assert
		using (new AssertionScope())
		{
			updated
				.Should().NotBeNull();
			updated.Id
				.Should().Be("12345");
			updated.FirstName
				.Should().Be("Updated");
			updated.LastName
				.Should().Be("Nventive");
			updated.Email
				.Should().Be("nventive@nventive.ca");
		}
	}
}
