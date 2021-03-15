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
	public partial class PostServiceShould
	{
		[Fact]
		public async Task GetAllPosts()
		{
			// Arrange
			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.GetAll(It.IsAny<CancellationToken>()))
				.ReturnsAsync(GetMockedPosts().ToArray());

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			var results = await sut.GetPosts(CancellationToken.None);

			// Assert
			results.Should().NotBeNullOrEmpty();
		}

		[Theory]
		[InlineAutoData(1)]
		[InlineAutoData(2)]
		public async Task GetPostWhenGivenIdIsValid(long givenId)
		{
			// Arrange
			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.Get(It.IsAny<CancellationToken>(), givenId))
				.ReturnsAsync(GetMockedPost(givenId));

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			var result = await sut.GetPost(CancellationToken.None, givenId);

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
			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.GetAll(It.IsAny<CancellationToken>()))
				.ReturnsAsync(GetMockedPosts().ToArray());

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			Func<Task<PostData>> act = () => sut.GetPost(CancellationToken.None, givenId);

			// Assert with invalid id
			await act
				.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>(because: "Id '{0}' is not registered in database", givenId);
		}

		[Fact]
		public async Task CreatePost()
		{
			// Arrange
			var post = new PostData.Builder()
			{
				Title = "My title",
				Body = "My body",
				UserIdentifier = 100,
			};

			var randomId = new Random().Next(1, int.MaxValue);

			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.Create(It.IsAny<CancellationToken>(), post))
				.ReturnsAsync(post.WithId(randomId).ToImmutable());

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			var result = await sut.Create(CancellationToken.None, post);

			// Assert
			using (new AssertionScope())
			{
				result
					.Should().NotBeNull();
				result.Id
					.Should().Be(randomId);
				result.Title
					.Should().Be(post.Title);
				result.Body
					.Should().Be(post.Body);
				result.UserIdentifier
					.Should().Be(post.UserIdentifier);
			}
		}

		[Fact]
		public async Task ReturnsNullWhenCreatePostFailed()
		{
			// Arrange
			var post = new PostData.Builder()
			{
				Title = "My title",
				Body = "My body",
				UserIdentifier = 100,
			};

			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.Create(It.IsAny<CancellationToken>(), post))
				.ReturnsAsync(default(PostData));

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			var result = await sut.Create(CancellationToken.None, post);

			// Assert
			result
				.Should().BeNull();
		}

		[Fact]
		public async Task ReturnsNullWhenCreatePostBodyIsNull()
		{
			// Arrange
			var post = default(PostData);

			var randomId = new Random().Next(1, int.MaxValue);

			// This part is the part that must be defined by the API contract.
			// Since there is none here, we are assuming it's giving us a null object when the body is null
			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.Create(It.IsAny<CancellationToken>(), post))
				.ReturnsAsync(default(PostData));

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			var result = await sut.Create(CancellationToken.None, post);

			// Assert
			result
				.Should().BeNull();
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

			// This part is the part that must be defined by the API contract.
			// Since there is none here, we are assuming it's giving us a null object when the body is null
			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.Update(It.IsAny<CancellationToken>(), post.Id, post))
				.ReturnsAsync(post);

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			var result = await sut.Update(CancellationToken.None, post.Id, post);

			// Assert
			using (new AssertionScope())
			{
				result
					.Should().NotBeNull();
				result.Id
					.Should().Be(post.Id);
				result.Title
					.Should().Be(post.Title);
				result.Body
					.Should().Be(post.Body);
				result.UserIdentifier
					.Should().Be(post.UserIdentifier);
			}
		}

		[Fact]
		public async Task DeletePostWhenGivenPostExists()
		{
			// Arrange
			var postId = 1;

			var mockedPostEndpoint = new Mock<IPostEndpoint>();
			mockedPostEndpoint
				.Setup(endpoint => endpoint.Delete(It.IsAny<CancellationToken>(), postId))
				.Returns(Task.CompletedTask);

			var sut = new PostService(mockedPostEndpoint.Object);

			// Act
			Func<Task> act = () => sut.Delete(CancellationToken.None, postId);

			// Assert
			await act
				.Should().NotThrowAsync();
		}
	}
}
