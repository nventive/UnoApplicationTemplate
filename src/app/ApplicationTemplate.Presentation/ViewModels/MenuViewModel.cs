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

public sealed class MenuViewModel : ViewModel
{
	public enum Section
	{
		Home,
		Posts,
		Settings,
		Agentic
	}

	private readonly ISectionsNavigator _sectionsNavigator;
	private readonly IReviewService _reviewService;

	public MenuViewModel()
	{
		ResolveService(out _sectionsNavigator);
		ResolveService(out _reviewService);
	}

	/// <summary>
	/// The list of ViewModel types on which the bottom menu should be visible.
	/// </summary>
	private static readonly Type[] _viewModelsWithBottomMenu = new[]
	{
		typeof(DadJokesPageViewModel),
		typeof(PostsPageViewModel),
		typeof(SettingsPageViewModel),
		typeof(ViewModels.Agentic.AgenticChatPageViewModel),
	};

	public string MenuState => this.GetFromObservable(
		ObserveMenuState(),
		initialValue: GetMenuState(_sectionsNavigator.State.GetCurrentOrNextViewModelType())
	);

	public int SelectedIndex => this.GetFromObservable<int>(ObserveSelectedIndex(), initialValue: 0);

	public IDynamicCommand ShowHomeSection => this.GetCommandFromTask(async ct =>
	{
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Home), () => new DadJokesPageViewModel());
		await _reviewService.TryRequestReview(ct);
	});

	public IDynamicCommand ShowPostsSection => this.GetCommandFromTask(async ct =>
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Posts), () => new PostsPageViewModel()));

	public IDynamicCommand ShowSettingsSection => this.GetCommandFromTask(async ct =>
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Settings), () => new SettingsPageViewModel()));

	public IDynamicCommand ShowAgenticSection => this.GetCommandFromTask(async ct =>
		await _sectionsNavigator.SetActiveSection(ct, nameof(Section.Agentic), () => new ViewModels.Agentic.AgenticChatPageViewModel()));

	private IObservable<string> ObserveMenuState() =>
		_sectionsNavigator
			.ObserveCurrentState()
			.Select(state =>
			{
				var vmType = state.GetCurrentOrNextViewModelType();
				return GetMenuState(vmType);
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
					nameof(Section.Agentic) => 3,
					_ => 0,
				};
			})
			.DistinctUntilChanged();

	private static string GetMenuState(Type viewModelType) => _viewModelsWithBottomMenu.Contains(viewModelType) ? "Open" : "Closed";
}
