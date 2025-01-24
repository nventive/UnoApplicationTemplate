using System;
using System.Reactive.Linq;
using ApplicationTemplate.Business;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;

namespace ApplicationTemplate.Presentation;

public sealed class DadJokesFiltersPageViewModel : ViewModel
{
	public IDynamicCommand HandleCheck => this.GetCommand((string pt) =>
	{
		PostTypeFilter = Enum.Parse<PostTypes>(pt, true);
	});

	public IDynamicCommand FiltersAndNavigate => this.GetCommandFromTask(async ct =>
	{
		this.GetService<IDadJokesService>().SetPostTypeFilter(PostTypeFilter);
		await this.GetService<ISectionsNavigator>().NavigateBack(ct);
	});

	public PostTypes PostTypeFilter
	{
		get => this.GetFromTask(ct => this.GetService<IDadJokesService>().GetAndObservePostTypeFilter().FirstAsync(ct), initialValue: PostTypes.Hot);
		set => this.Set(value);
	}
}
