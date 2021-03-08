using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class PostServiceShould : TestBase<IPostService>
	{
		[Fact]
		public async Task GetAllPosts()
		{
			// Act
			var results = await SUT.GetPosts(DefaultCancellationToken);

			// Assert
			results.Should().NotBeNullOrEmpty();
		}

		[Theory]
		// Valid cases
		[InlineAutoData(1)]
		// Invalid cases
		[InlineAutoData(-1)]
		[InlineAutoData(0)]
		[InlineAutoData(int.MaxValue)] // This will be invalid most of the time but it can be valid if the max amount of posts if created.
		public async Task GetPostOrThrowException(int givenId)
		{
			// Arrange
			var posts = await SUT.GetPosts(DefaultCancellationToken);
			var postsNumber = posts.Count;

			// Act
			Func<Task<PostData>> act = () => SUT.GetPost(DefaultCancellationToken, givenId);

			if (givenId >= 1 && givenId <= postsNumber)
			{
				var result = await act();

				// Assert with valid id
				result
					.Should().NotBeNull();
				result.Id
					.Should().Be(givenId);
			}
			else
			{
				// Assert with invalid id
				await act
					.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>(because: "Id '{0}' is not registered in database", givenId);
			}
		}

		[Fact]
		public async Task CreatePostOrThrowException()
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
		public async Task UpdateWhenGivenPostAlreadyExists()
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
		public async Task DeletePostWhenGivenPostExists()
		{
			// Arrange
			var postId = 1;

			// Act
			await SUT.Delete(DefaultCancellationToken, postId);
		}
	}
}
