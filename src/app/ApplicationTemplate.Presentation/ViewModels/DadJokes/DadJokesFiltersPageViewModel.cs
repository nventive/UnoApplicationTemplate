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
		get => this.GetFromTask<PostTypes>(ct => this.GetService<IDadJokesService>().GetAndObservePostTypeFilter().FirstAsync(ct), initialValue: PostTypes.Hot);
		set => this.Set(value);
	}
}
