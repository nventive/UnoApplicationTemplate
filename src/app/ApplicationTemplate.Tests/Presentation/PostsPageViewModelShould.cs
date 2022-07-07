﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ApplicationTemplate.Client;
using ApplicationTemplate.Presentation;
using Chinook.DynamicMvvm;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ApplicationTemplate.Tests;

public partial class PostsPageViewModelShould : NavigationTestsBase
{
	[Fact]
	public async Task GetAll()
	{
		// Arrange
		await NavigateAndClear(DefaultCancellationToken, () => new PostsPageViewModel());
		var viewModel = (PostsPageViewModel)GetCurrentViewModel();

		// Act
		var posts = await viewModel.Posts.Load(DefaultCancellationToken) as ImmutableList<PostData>;

		// Assert
		posts.Should().NotBeNull();
		posts.Should().HaveCount(100);
	}

	[Fact]
	public async Task NavigateToEditPost()
	{
		// Arrange
		await NavigateAndClear(DefaultCancellationToken, () => new PostsPageViewModel());
		var viewModel = (PostsPageViewModel)GetCurrentViewModel();
		var posts = await viewModel.Posts.Load(DefaultCancellationToken) as ImmutableList<PostData>;

		// Act
		var editedPost = posts.First();
		await viewModel.NavigateToPost.Execute(editedPost);
		var currentViewModel = GetCurrentViewModel();

		// Assert
		using (new AssertionScope())
		{
			currentViewModel.Should().NotBeNull();
			currentViewModel.Should().BeOfType<EditPostPageViewModel>();
		}

		// Assert the following only if we confirmed the currentViewModel is valid
		using (new AssertionScope())
		{
			var editPostVm = currentViewModel as EditPostPageViewModel;
			editPostVm.Title.Should().Be(editedPost.Title);
			editPostVm.IsNewPost.Should().BeFalse();
		}
	}
}
