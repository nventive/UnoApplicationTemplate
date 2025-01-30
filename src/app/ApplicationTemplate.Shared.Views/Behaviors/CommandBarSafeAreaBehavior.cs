using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ApplicationTemplate.Views.Behaviors;

public class CommandBarSafeAreaBehavior
{
	public static bool GetApplySafeAreaWorkaround(DependencyObject obj) => (bool)obj.GetValue(ApplySafeAreaWorkaroundProperty);

	public static void SetApplySafeAreaWorkaround(DependencyObject obj, bool value) => obj.SetValue(ApplySafeAreaWorkaroundProperty, value);

	public static readonly DependencyProperty ApplySafeAreaWorkaroundProperty =
		DependencyProperty.RegisterAttached(
			"ApplySafeAreaWorkaround",
			typeof(bool),
			typeof(CommandBarSafeAreaBehavior),
			new PropertyMetadata(false, OnPropertyChanged));

	private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is CommandBar commandBar && (bool)e.NewValue)
		{
#if ANDROID
			commandBar.Loaded += (s, args) =>
			{
				// We only need to apply the workaround on Android other platforms work well without it.
				// See this issue:https://github.com/unoplatform/uno/issues/6218
				var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height;
				commandBar.Margin = new Thickness(0, statusBar, 0, 0);
			};
#endif
		}
	}
}
