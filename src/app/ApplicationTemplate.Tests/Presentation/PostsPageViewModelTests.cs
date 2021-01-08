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
			await StartNavigation(DefaultCancellationToken, () => new PostsPageViewModel());
			var viewModel = (PostsPageViewModel)GetCurrentViewModel();
			var posts = await viewModel.Posts.Load(DefaultCancellationToken) as ImmutableList<PostData>;

			posts.Should().NotBeNull();
			posts.Should().HaveCount(100);
		}

		[Fact]
		public async Task It_Should_Navigate_New_Post()
		{
			await StartNavigation(DefaultCancellationToken, () => new PostsPageViewModel());
			var viewModel = (PostsPageViewModel)GetCurrentViewModel();
			await viewModel.NavigateToNewPost.Execute();
			var currentViewModel = GetCurrentViewModel();

			currentViewModel.Should().NotBeNull();
			currentViewModel.Should().BeOfType<EditPostPageViewModel>();

			var editPostVm = currentViewModel as EditPostPageViewModel;
			editPostVm.IsNewPost.Should().BeTrue();
		}

		[Fact]
		public async Task It_Should_Navigate_Edit_Post()
		{
			await StartNavigation(DefaultCancellationToken, () => new PostsPageViewModel());
			var viewModel = (PostsPageViewModel)GetCurrentViewModel();
			var posts = await viewModel.Posts.Load(DefaultCancellationToken) as ImmutableList<PostData>;
			var editedPost = posts.First();
			await viewModel.NavigateToPost.Execute(editedPost);
			var currentViewModel = GetCurrentViewModel();

			currentViewModel.Should().NotBeNull();
			currentViewModel.Should().BeOfType<EditPostPageViewModel>();

			var editPostVm = currentViewModel as EditPostPageViewModel;
			editPostVm.Title.Should().Be(editedPost.Title);
			editPostVm.IsNewPost.Should().BeFalse();
		}
	}
}
