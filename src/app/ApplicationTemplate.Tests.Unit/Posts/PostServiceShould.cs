﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.DataAccess;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Execution;
using NSubstitute;
using Xunit;

namespace ApplicationTemplate.Tests;

public sealed partial class PostServiceShould
{
	[Fact]
	public async Task GetAllPosts()
	{
		// Arrange
		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.GetAll(Arg.Any<CancellationToken>())
			.Returns(Task.FromResult(GetMockedPosts().ToArray()));

		var sut = new PostService(mockedPostsRepository);

		// Act
		var results = await sut.GetPosts(CancellationToken.None);

		// Assert
		results.Should().NotBeNullOrEmpty();
	}

	[Theory]
	[InlineAutoData(1)]
	[InlineAutoData(2)]
	public async Task GetPost_WhenGivenIdIsValid(long givenId)
	{
		// Arrange
		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Get(Arg.Any<CancellationToken>(), givenId)
			.Returns(Task.FromResult(GetMockedPost(givenId)));

		var sut = new PostService(mockedPostsRepository);

		// Act
		var result = await sut.GetPost(CancellationToken.None, givenId);

		// Assert with valid id
		using (new AssertionScope())
		{
			result
				.Should().NotBeNull();
			result
				.Should().BeOfType<Post>();
			result.Id
				.Should().Be(givenId);
		}
	}

	[Theory]
	[InlineAutoData(-1)]
	[InlineAutoData(0)]
	[InlineAutoData(int.MaxValue)] // This will be invalid most of the time but it can be valid if the max amount of posts if created.
	public async Task GetPostThrowException_WhenGivenIdIsInvalid(int givenId)
	{
		// Arrange
		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Get(Arg.Any<CancellationToken>(), Arg.Any<long>())
			.Returns(Task.FromResult(GetMockedPosts()
				.Where(post => post.Id == givenId)
				.FirstOrDefault())
			);

		var sut = new PostService(mockedPostsRepository);

		// Act
		var result = await sut.GetPost(CancellationToken.None, givenId);

		// Assert with invalid id
		result
			.Should()
			.Be(default(Post));
	}

	[Fact]
	public async Task CreatePost()
	{
		// Arrange
		var post = new Post()
		{
			Title = "My title",
			Body = "My body",
			UserIdentifier = 100
		};

		var randomId = new Random().Next(1, int.MaxValue);

		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Create(Arg.Any<CancellationToken>(), Arg.Any<PostData>())
			.Returns(Task.FromResult((post with { Id = randomId }).ToData()));

		var sut = new PostService(mockedPostsRepository);

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
	public async Task ReturnNull_WhenCreatePostFailed()
	{
		// Arrange
		var post = new Post { Title = "My title", Body = "My body", UserIdentifier = 100 };

		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Create(Arg.Any<CancellationToken>(), post.ToData())
			.Returns(Task.FromResult(default(PostData)));

		var sut = new PostService(mockedPostsRepository);

		// Act
		var result = await sut.Create(CancellationToken.None, post);

		// Assert
		result
			.Should().BeNull();
	}

	[Fact]
	public async Task ReturnNull_WhenCreatePostBodyIsNull()
	{
		// Arrange
		var post = default(Post);

		var randomId = new Random().Next(1, int.MaxValue);

		// This part is the part that must be defined by the API contract.
		// Since there is none here, we are assuming it's giving us a null object when the body is null
		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Create(Arg.Any<CancellationToken>(), post.ToData())
			.Returns(Task.FromResult(default(PostData)));

		var sut = new PostService(mockedPostsRepository);

		// Act
		var result = await sut.Create(CancellationToken.None, post);

		// Assert
		result
			.Should().BeNull();
	}

	[Fact]
	public async Task Update_WhenGivenPostAlreadyExists()
	{
		// Arrange
		var post = new Post { Id = 1, Title = "My updated title", Body = "My updated body", UserIdentifier = 100 };

		// This part is the part that must be defined by the API contract.
		// Since there is none here, we are assuming it's giving us a null object when the body is null
		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Update(Arg.Any<CancellationToken>(), post.Id, post.ToData())
			.Returns(Task.FromResult(post.ToData()));

		var sut = new PostService(mockedPostsRepository);

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
	public async Task DeletePost_WhenGivenPostExists()
	{
		// Arrange
		var postId = 1;

		var mockedPostsRepository = Substitute.For<IPostsRepository>();
		mockedPostsRepository
			.Delete(Arg.Any<CancellationToken>(), postId)
			.Returns(Task.CompletedTask);

		var sut = new PostService(mockedPostsRepository);

		// Act
		Func<Task> act = () => sut.Delete(CancellationToken.None, postId);

		// Assert
		await act
			.Should().NotThrowAsync();
	}
}
