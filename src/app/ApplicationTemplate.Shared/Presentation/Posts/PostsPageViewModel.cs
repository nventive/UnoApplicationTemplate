using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Client;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation
{
	public partial class PostsPageViewModel : ViewModel
	{
		private readonly Func<Task> _onGetPostsCalled;  

		private readonly ManualDataLoaderTrigger _deletePostTrigger = new ManualDataLoaderTrigger();

		public PostsPageViewModel(Func<Task> onGetPostsCalled = null)
		{
			_onGetPostsCalled = onGetPostsCalled;
		}

		public IDynamicCommand DeletePost => this.GetCommandFromTask<PostData>(async (ct, post) =>
		{
			await this.GetService<IPostService>().Delete(ct, post.Id);
		});

		public IDynamicCommand NavigateToNewPost => this.GetCommandFromTask(async ct =>
		{
			await this.GetService<ISectionsNavigator>().OpenModal(ct, () => new EditPostPageViewModel());
		});

		public IDynamicCommand NavigateToPost => this.GetCommandFromTask<PostData>(async (ct, post) =>
		{
			await this.GetService<ISectionsNavigator>().Navigate(ct, () => new EditPostPageViewModel(post));
		});

		public IDynamicCommand RefreshPosts => this.GetCommandFromDataLoaderRefresh(Posts);

		public IDataLoader Posts => this.GetDataLoader(GetPosts, d => d.WithTrigger(_deletePostTrigger));

		private async Task<ImmutableList<PostData>> GetPosts(CancellationToken ct)
		{
			if (_onGetPostsCalled != null)
			{
				await _onGetPostsCalled();
			}

			return await this.GetService<IPostService>().GetPosts(ct);
		}
	}
}
