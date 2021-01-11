using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinook.DynamicMvvm;
using FluentAssertions;
using Xunit;

namespace ApplicationTemplate.Tests
{
	public class PostsPageViewModelTests : NavigationTestsBase
	{
		[Fact]
		public async Task It_Should_GetAll()
		{
			// Arrange
			await StartNavigation(DefaultCancellationToken, () => new PostsPageViewModel());
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
			// Arrangee
			await StartNavigation(DefaultCancellationToken, () => new PostsPageViewModel());
			var viewModel = (PostsPageViewModel)GetCurrentViewModel();

			// Act
			await viewModel.NavigateToNewPost.Execute();

			// Assert
			var currentViewModel = GetCurrentViewModel();
			currentViewModel.Should().NotBeNull();
			currentViewModel.Should().BeOfType<EditPostPageViewModel>();

			var editPostVm = currentViewModel as EditPostPageViewModel;
			editPostVm.IsNewPost.Should().BeTrue();
		}

		[Fact]
		public async Task It_Should_Navigate_Edit_Post()
		{
			// Arrange
			await StartNavigation(DefaultCancellationToken, () => new PostsPageViewModel());
			var viewModel = (PostsPageViewModel)GetCurrentViewModel();
			var posts = await viewModel.Posts.Load(DefaultCancellationToken) as ImmutableList<PostData>;

			// Act
			var editedPost = posts.First();
			await viewModel.NavigateToPost.Execute(editedPost);
			var currentViewModel = GetCurrentViewModel();

			// Assert
			currentViewModel.Should().NotBeNull();
			currentViewModel.Should().BeOfType<EditPostPageViewModel>();

			var editPostVm = currentViewModel as EditPostPageViewModel;
			editPostVm.Title.Should().Be(editedPost.Title);
			editPostVm.IsNewPost.Should().BeFalse();
		}
	}
}
