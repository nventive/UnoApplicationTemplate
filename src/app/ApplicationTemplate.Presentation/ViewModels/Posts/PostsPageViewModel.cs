using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DataLoader;
using Chinook.DynamicMvvm;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation;

public sealed class PostsPageViewModel : ViewModel
{
	[SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "It will be disposed by the DataLoader when passed via WithTrigger.")]
	private readonly ManualDataLoaderTrigger _deletePostTrigger = new();

	public IDynamicCommand CreatePost => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<IStackNavigator>().Navigate(ct, () => new EditPostPageViewModel());
	});

	public IDataLoader Posts => this.GetDataLoader(GetPosts, d => d.WithTrigger(_deletePostTrigger));

	private async Task<ImmutableList<PostItemViewModel>> GetPosts(CancellationToken ct)
	{
		var posts = await this.GetService<IPostService>().GetPosts(ct);

		return posts
			.Select(p => this.GetChild(() => new PostItemViewModel(p), p.Id.ToString(CultureInfo.InvariantCulture)))
			.ToImmutableList();
	}
}
