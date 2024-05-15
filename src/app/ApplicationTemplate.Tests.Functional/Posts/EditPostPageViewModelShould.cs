using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
		var postItemViewModels = await postsVM.Posts.Load(CancellationToken.None) as ImmutableList<PostItemViewModel>;

		var postItemViewModel = postItemViewModels.First();
		var post = postItemViewModel.Post;

		await postItemViewModel.EditPost.Execute();
		var sut = GetAndAssertActiveViewModel<EditPostPageViewModel>();

		// Assert
		sut.Title.Should().Be(post.Title);
		sut.IsNewPost.Should().BeFalse();

		sut.Form.Title.Should().Be(post.Title);
		sut.Form.Body.Should().Be(post.Body);
	}
}
