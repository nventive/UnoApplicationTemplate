using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Presentation;
using Chinook.DynamicMvvm;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Xunit.Abstractions;

namespace ApplicationTemplate.Tests;

public sealed class EditPostPageViewModelShould : FunctionalTestBase
{
	public EditPostPageViewModelShould(ITestOutputHelper output) : base(output)
	{
	}

	[Fact]
	public async Task ShowPostDetails()
	{
		// Arrange & Act
		await this.Login(await this.ReachLoginPage());
		await Menu.ShowPostsSection.Execute();

		var postsVM = GetAndAssertActiveViewModel<PostsPageViewModel>();
		var posts = await postsVM.Posts.Load(CancellationToken.None) as ImmutableList<Post>;

		var post = posts.First();
		await postsVM.NavigateToPost.Execute(post);
		var sut = GetAndAssertActiveViewModel<EditPostPageViewModel>();

		// Assert
		sut.Title.Should().Be(post.Title);
		sut.IsNewPost.Should().BeFalse();

		sut.Form.Title.Should().Be(post.Title);
		sut.Form.Body.Should().Be(post.Body);
	}
}
