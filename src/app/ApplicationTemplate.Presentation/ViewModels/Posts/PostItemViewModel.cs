using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation;

/// <summary>
/// Post item view model.
/// </summary>
public sealed class PostItemViewModel : ViewModel
{
	/// <summary>
	/// Initializes a new instance of the <see cref="PostItemViewModel"/> class.
	/// </summary>
	/// <param name="post">The post.</param>
	public PostItemViewModel(Post post)
	{
		Post = post;
	}

	/// <summary>
	/// The post.
	/// </summary>
	public Post Post { get; }

	/// <summary>
	/// Deletes the post.
	/// </summary>
	public IDynamicCommand DeletePost => this.GetCommandFromTask(async (ct) =>
	{
		await this.GetService<IPostService>().Delete(ct, Post.Id);
	});

	/// <summary>
	/// Navigates to the edit post page.
	/// </summary>
	public IDynamicCommand EditPost => this.GetCommandFromTask(async (ct) =>
	{
		await this.GetService<IStackNavigator>().Navigate(ct, () => new EditPostPageViewModel(Post));
	});
}
