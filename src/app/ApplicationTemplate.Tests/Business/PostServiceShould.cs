using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Xunit;

namespace ApplicationTemplate.Tests.Business
{
	public partial class PostServiceShould : TestBase<IPostService>
	{
		private Mock<IPostEndpoint> _mockedPostEndpoint;
		private PostService _sut;

		public PostServiceShould()
		{
			_mockedPostEndpoint = new Mock<IPostEndpoint>();
			_sut = new PostService(_mockedPostEndpoint.Object);
		}

		[Fact]
		public async Task GetAllPosts()
		{
			// Act
			var results = await SUT.GetPosts(DefaultCancellationToken);

			// Assert
			results.Should().NotBeNullOrEmpty();
		}

		[Theory]
		[InlineAutoData(1)]
		[InlineAutoData(2)]
		public async Task GetPostWhenGivenIdIsValid(int givenId)
		{
			// Arrange
			_mockedPostEndpoint.Reset();
			_mockedPostEndpoint
				.Setup(endpoint => endpoint.GetAll(It.IsAny<CancellationToken>()))
				.ReturnsAsync(GetMockedPostData().ToArray());

			// Act
			Func<Task<PostData>> act = () => SUT.GetPost(DefaultCancellationToken, givenId);

			var result = await act();

			// Assert with valid id
			using (new AssertionScope())
			{
				result
					.Should().NotBeNull();
				result
					.Should().BeOfType<PostData>();
				result.Id
					.Should().Be(givenId);
			}
		}

		[Theory]
		[InlineAutoData(-1)]
		[InlineAutoData(0)]
		[InlineAutoData(int.MaxValue)] // This will be invalid most of the time but it can be valid if the max amount of posts if created.
		public async Task GetPostThrowExceptionWhenGivenIdIsInvalid(int givenId)
		{
			// Arrange
			_mockedPostEndpoint.Reset();
			_mockedPostEndpoint
				.Setup(endpoint => endpoint.GetAll(It.IsAny<CancellationToken>()))
				.ReturnsAsync(GetMockedPostData().ToArray());

			// Act
			Func<Task<PostData>> act = () => SUT.GetPost(DefaultCancellationToken, givenId);

			// Assert with invalid id
			await act
				.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>(because: "Id '{0}' is not registered in database", givenId);
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
