using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation;

public partial class PostsPageViewModel : ViewModel
{
	private readonly Func<Task> _onGetPostsCalled;

	private readonly ManualDataLoaderTrigger _deletePostTrigger = new();

	public PostsPageViewModel(Func<Task> onGetPostsCalled = null)
	{
		_onGetPostsCalled = onGetPostsCalled;
	}

	public IDynamicCommand DeletePost => this.GetCommandFromTask<Post>(async (ct, post) =>
	{
		await this.GetService<IPostService>().Delete(ct, post.Id);
	});

	public IDynamicCommand NavigateToNewPost => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<IStackNavigator>().Navigate(ct, () => new EditPostPageViewModel());
	});

	public IDynamicCommand NavigateToPost => this.GetCommandFromTask<Post>(async (ct, post) =>
	{
		await this.GetService<IStackNavigator>().Navigate(ct, () => new EditPostPageViewModel(post));
	});

	public IDataLoader Posts => this.GetDataLoader(GetPosts, d => d.WithTrigger(_deletePostTrigger));

	private async Task<ImmutableList<Post>> GetPosts(CancellationToken ct)
	{
		if (_onGetPostsCalled != null)
		{
			await _onGetPostsCalled();
		}

		return await this.GetService<IPostService>().GetPosts(ct);
	}
}
