using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;

namespace ApplicationTemplate.Presentation;

public class DadJokesFiltersPageViewModel : ViewModel
{
	public DadJokesFiltersPageViewModel()
	{
		var pt = this.GetService<IDadJokesService>().GetAndObservePostTypeFilter();
		var postType = GetPostType(pt);

		PostTypeFilter = postType.Result;

		async Task<PostTypes> GetPostType(ReplaySubject<PostTypes> ptArg)
		{
			return await ptArg.FirstAsync();
		}
	}

	public IDynamicCommand HandleCheck => this.GetCommand((string pt) =>
	{
		PostTypeFilter = (PostTypes)Enum.Parse(typeof(PostTypes), pt, true);
	});

	public IDynamicCommand FiltersAndNavigate => this.GetCommandFromTask(async ct =>
	{
		this.GetService<IDadJokesService>().SetPostTypeFilter(PostTypeFilter);
		await this.GetService<ISectionsNavigator>().NavigateBack(ct);
	});

	public PostTypes PostTypeFilter
	{
		get => this.Get<PostTypes>(initialValue: PostTypes.Hot);
		set => this.Set(value);
	}
}
