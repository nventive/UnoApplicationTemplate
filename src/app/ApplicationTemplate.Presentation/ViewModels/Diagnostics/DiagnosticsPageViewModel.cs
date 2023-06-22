using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Chinook.StackNavigation;
using Uno;

namespace ApplicationTemplate.Presentation;

public sealed partial class DiagnosticsPageViewModel : ViewModel
{
	[Inject] private IEnvironmentManager _environmentManager;

	public IViewModel SummaryDiagnostics => this.GetChild<SummaryDiagnosticsViewModel>();

	public IViewModel ExceptionDiagnostics => this.GetChild<ExceptionsDiagnosticsViewModel>();

	public IViewModel CultureDiagnostics => this.GetChild<CultureDiagnosticsViewModel>();

	public IViewModel LoggersDiagnostics => this.GetChild<LoggersDiagnosticsViewModel>();

	public IViewModel SettingsDiagnostics => this.GetChild<SettingsDiagnosticsViewModel>();

	public IDynamicCommand NavigateToEnvironmentPickerPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new EnvironmentPickerPageViewModel(CurrentEnvironment));
	});

	public IDynamicCommand NavigateToStarRatingTestingPage => this.GetCommandFromTask(async ct =>
	{
		await this.GetService<ISectionsNavigator>().Navigate(ct, () => new StarRatingTestingPageViewModel());
	});

	public string CurrentEnvironment
	{
		get => this.Get(initialValue: _environmentManager.Current);
		private set => this.Set(value);
	}
}
