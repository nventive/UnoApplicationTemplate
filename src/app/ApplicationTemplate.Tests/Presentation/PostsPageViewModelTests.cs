﻿using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public partial class PostsPageViewModelTests : NavigationTestsBase
	{
		[Fact]
		public async Task It_Should_GetAll()
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
		public async Task It_Should_Navigate_New_Post()
		{
			// Arrange
			await NavigateAndClear(DefaultCancellationToken, () => new PostsPageViewModel());
			var viewModel = (PostsPageViewModel)GetCurrentViewModel();

			// Act
			await viewModel.NavigateToNewPost.Execute();

			// Assert
			var currentViewModel = GetCurrentViewModel();
			using (new AssertionScope())
			{
				currentViewModel.Should().NotBeNull();
				currentViewModel.Should().BeOfType<EditPostPageViewModel>();
			}

			using (new AssertionScope())
			{
				var editPostVm = currentViewModel as EditPostPageViewModel;
				editPostVm.IsNewPost.Should().BeTrue();
			}
		}

		[Fact]
		public async Task It_Should_Navigate_Edit_Post()
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
}
