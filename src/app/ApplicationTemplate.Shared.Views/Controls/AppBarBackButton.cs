using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Chinook.DynamicMvvm;
using Chinook.SectionsNavigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Controls;

/// <summary>
/// Back button for Windows.
/// Must be used with <see cref="Uno.UI.Toolkit.CommandBarExtensions.NavigationCommandProperty"/>.
/// </summary>
/// <remarks>
/// We could investigate to see if it could be made with <see cref="Chinook.BackButtonManager.IBackButtonSource"/> or in a better way.
/// </remarks>
public sealed partial class AppBarBackButton : AppBarButton
{
	private IDisposable _backButtonVisibilitySubscription;

	public AppBarBackButton()
	{
		Icon = new FontIcon()
		{
			// Value in XAML (&#xE72B).
			Glyph = "\xE72B",
		};
		Command = BuildClickCommand();

		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
	}

	private static IServiceProvider ServiceProvider => ViewModelBase.DefaultServiceProvider;

	private static ISectionsNavigator SectionsNavigator => ServiceProvider.GetRequiredService<ISectionsNavigator>();

	private static IDynamicCommandBuilderFactory CommandBuilderFactory { get; } = new DynamicCommandBuilderFactory(c => c
		.CatchErrors(ServiceProvider.GetRequiredService<IDynamicCommandErrorHandler>())
		.WithLogs(ServiceProvider.GetRequiredService<ILogger<IDynamicCommand>>())
		.DisableWhileExecuting()
	);

	private static ICommand BuildClickCommand() => CommandBuilderFactory
		.CreateFromTask("Click", async (ct) => await SectionsNavigator.NavigateBackOrCloseModal(ct))
		.Build();

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		_backButtonVisibilitySubscription = ObserveBackButtonVisibility();
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
		_backButtonVisibilitySubscription.Dispose();
	}

	private IDisposable ObserveBackButtonVisibility()
	{
		var multiNavigationController = SectionsNavigator;

		return Observable
			.FromEventPattern<SectionsNavigatorStateChangedEventHandler, SectionsNavigatorEventArgs>(
				h => multiNavigationController.StateChanged += h,
				h => multiNavigationController.StateChanged -= h
			)
			.Select(_ => multiNavigationController.CanNavigateBackOrCloseModal())
			.StartWith(multiNavigationController.CanNavigateBackOrCloseModal())
			.DistinctUntilChanged()
			.Subscribe(OnStateChanged);

		void OnStateChanged(bool canNavigateBackOrCloseModal)
		{
			var dispatcherQueue = ServiceProvider.GetRequiredService<DispatcherQueue>();
			_ = dispatcherQueue.RunAsync(DispatcherQueuePriority.Normal, UpdateBackButtonUI);

			void UpdateBackButtonUI() // Runs on UI thread.
			{
				Visibility = canNavigateBackOrCloseModal ? Visibility.Visible : Visibility.Collapsed;
			}
		}
	}
}
