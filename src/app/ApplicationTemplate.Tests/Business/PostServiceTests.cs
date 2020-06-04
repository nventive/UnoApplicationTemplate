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
			var results = await SUT.GetPosts(DefaultCancellationToken);

			results.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task It_Should_GetOne()
		{
			var postId = 1;
			var result = await SUT.GetPost(DefaultCancellationToken, postId);

			result.Should().NotBeNull();
			result.Id.Should().Be(postId);
		}

		[Fact]
		public async Task It_Should_Create()
		{
			var post = new PostData.Builder()
			{
				Title = "My title",
				Body = "My body",
				UserIdentifier = 100,
			};

			var result = await SUT.Create(DefaultCancellationToken, post);

			result.Should().NotBeNull();
			result.Id.Should().BeGreaterThan(0);
			result.Title.Should().Be(post.Title);
			result.Body.Should().Be(post.Body);
			result.UserIdentifier.Should().Be(post.UserIdentifier);
		}

		[Fact]
		public async Task It_Should_Update()
		{
			var post = new PostData.Builder()
			{
				Id = 1,
				Title = "My updated title",
				Body = "My updated body",
				UserIdentifier = 100,
			};

			var result = await SUT.Update(DefaultCancellationToken, post.Id, post);

			result.Should().NotBeNull();
			result.Id.Should().Be(post.Id);
			result.Title.Should().Be(post.Title);
			result.Body.Should().Be(post.Body);
			result.UserIdentifier.Should().Be(post.UserIdentifier);
		}

		[Fact]
		public async Task It_Should_Delete()
		{
			var postId = 1;

			await SUT.Delete(DefaultCancellationToken, postId);
		}
	}
}
