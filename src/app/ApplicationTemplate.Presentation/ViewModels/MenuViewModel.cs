using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Presentation;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Uno;
using Uno.Extensions;

namespace ApplicationTemplate.Presentation;

public partial class MenuViewModel : ViewModel
{
	public enum Section
	{
		Home,
		Posts,
		Settings
	}

	[Inject] private ISectionsNavigator _navigator;

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

	public int SelectedIndex
	{
		get => this.Get<int>(initialValue: 0);
		set => this.Set(value);
	}

	public IDynamicCommand ShowHomeSection => this.GetCommandFromTask(async ct =>
		await _navigator.SetActiveSection(ct, nameof(Section.Home), () => new DadJokesPageViewModel()));

	public IDynamicCommand ShowPostsSection => this.GetCommandFromTask(async ct =>
		await _navigator.SetActiveSection(ct, nameof(Section.Posts), () => new PostsPageViewModel()));

	public IDynamicCommand ShowSettingsSection => this.GetCommandFromTask(async ct =>
		await _navigator.SetActiveSection(ct, nameof(Section.Settings), () => new SettingsPageViewModel()));

	private IObservable<string> ObserveMenuState() =>
		_navigator
			.ObserveCurrentState()
			.Select(state =>
			{
				var vmType = state.GetViewModelType();
				SelectedIndex = _viewModelsWithBottomMenu.IndexOf(vmType);
				return _viewModelsWithBottomMenu.Contains(vmType) ? "Open" : "Closed";
			})
			.DistinctUntilChanged()
			// On iOS, when Visual states are changed too fast, they break. This is a workaround for this bug.
			.ThrottleOrImmediate(TimeSpan.FromMilliseconds(350), Scheduler.Default);
}
