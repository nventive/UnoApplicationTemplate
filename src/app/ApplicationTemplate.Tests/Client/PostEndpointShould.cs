using System.Threading.Tasks;
using ApplicationTemplate.Client;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests;

public class PostEndpointShould : IntegrationTestBase<IPostEndpoint>
{
	[Fact]
	public async Task GetAll()
	{
		// Act
		var results = await SUT.GetAll(DefaultCancellationToken);

		// Assert
		results.Should().NotBeNullOrEmpty();
	}

	[Fact]
	public async Task GetOne()
	{
		// Arrange
		var postId = 1;

		// Act
		var result = await SUT.Get(DefaultCancellationToken, postId);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(postId);
	}

	[Fact]
	public async Task CreateOne()
	{
		// Arrange
		var post = new PostData.Builder()
		{
			Title = "My title",
			Body = "My body",
			UserIdentifier = 100,
		};

		// Act
		var result = await SUT.Create(DefaultCancellationToken, post);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().BeGreaterThan(0);
		result.Title.Should().Be(post.Title);
		result.Body.Should().Be(post.Body);
		result.UserIdentifier.Should().Be(post.UserIdentifier);
	}

	[Fact]
	public async Task UpdateOne()
	{
		// Arrange
		var post = new PostData.Builder()
		{
			Id = 1,
			Title = "My updated title",
			Body = "My updated body",
			UserIdentifier = 100,
		};

		// Act
		var result = await SUT.Update(DefaultCancellationToken, post.Id, post);

		// Assert
		result.Should().NotBeNull();
		result.Id.Should().Be(post.Id);
		result.Title.Should().Be(post.Title);
		result.Body.Should().Be(post.Body);
		result.UserIdentifier.Should().Be(post.UserIdentifier);
	}

	[Fact]
	public async Task DeleteOne()
	{
		// Arrange
		var postId = 1;

		// Act
		await SUT.Delete(DefaultCancellationToken, postId);
	}
}
