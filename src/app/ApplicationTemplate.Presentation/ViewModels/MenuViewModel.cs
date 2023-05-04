using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Uno;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation;

public sealed partial class MenuViewModel : ViewModel
{
	public enum Section
	{
		Home,
		Posts,
		Settings
	}

	[Inject] private ISectionsNavigator _sectionsNavigator;

	/// <summary>
	/// The list of ViewModel types on which the bottom menu should be visible.
	/// </summary>
	private Type[] _viewModelsWithBottomMenu = new Type[]
	{
		typeof(DadJokesPageViewModel),
		typeof(PostsPageViewModel),
		typeof(SettingsPageViewModel),
	};

	public string MenuState => this.GetFromObservable(ObserveMenuState(), initialValue: "Closed");

	public int SelectedIndex => this.GetFromObservable<int>(ObserveSelectedIndex(), initialValue: 0);

	public IDynamicCommand ShowHomeSection => this.GetCommandFromTask(async ct =>
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Home), () => new DadJokesPageViewModel()));

	public IDynamicCommand ShowPostsSection => this.GetCommandFromTask(async ct =>
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Posts), () => new PostsPageViewModel()));

	public IDynamicCommand ShowSettingsSection => this.GetCommandFromTask(async ct =>
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Settings), () => new SettingsPageViewModel()));

	private IObservable<string> ObserveMenuState() =>
		_sectionsNavigator
			.ObserveCurrentState()
			.Select(state =>
			{
				var vmType = state.GetViewModelType();
				return _viewModelsWithBottomMenu.Contains(vmType) ? "Open" : "Closed";
			})
			.DistinctUntilChanged()
			// On iOS, when Visual states are changed too fast, they break. This is a workaround for this bug.
			.ThrottleOrImmediate(TimeSpan.FromMilliseconds(350), Scheduler.Default);

	private IObservable<int> ObserveSelectedIndex() =>
		_sectionsNavigator
			.ObserveCurrentState()
			.Select(state =>
			{
				var sectionName = state.ActiveSection?.Name;
				return sectionName switch
				{
					nameof(Section.Home) => 0,
					nameof(Section.Posts) => 1,
					nameof(Section.Settings) => 2,
					_ => 0,
				};
			})
			.DistinctUntilChanged();
}
