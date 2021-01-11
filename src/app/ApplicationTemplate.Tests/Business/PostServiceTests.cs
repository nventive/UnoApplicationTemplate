using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class PostServiceTests : TestBase<IPostService>
	{
		[Fact]
		public async Task It_Should_GetAll()
		{
			// Act
			var results = await SUT.GetPosts(DefaultCancellationToken);

			// Assert
			results.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task It_Should_GetOne()
		{
			// Arrange
			var postId = 1;

			// Act
			var result = await SUT.GetPost(DefaultCancellationToken, postId);

			// Assert
			result.Should().NotBeNull();
			result.Id.Should().Be(postId);
		}

		[Fact]
		public async Task It_Should_Create()
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
		public async Task It_Should_Update()
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
		public async Task It_Should_Delete()
		{
			// Arrange
			var postId = 1;

			// Act
			await SUT.Delete(DefaultCancellationToken, postId);
		}
	}
}
