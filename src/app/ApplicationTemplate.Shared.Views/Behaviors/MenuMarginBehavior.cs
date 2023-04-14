using Microsoft.UI.Xaml;

namespace ApplicationTemplate.Views.Behaviors
{
	/// <summary>
	/// This behaviors adjusts the margin of the control to set the height of the menu
	/// when IsMarginEnabled == true. This way the page content is not hidden by the menu.
	/// </summary>
	public class MenuMarginBehavior
	{
		/// <summary>
		/// The height of the tabbar is actually set in menu.xaml
		/// Look at the row definitions.
		/// </summary>
		private const int TabBarHeight = 62;

		public static bool GetIsMarginEnabled(DependencyObject obj) => (bool)obj.GetValue(IsMarginEnabledProperty);

		public static void SetIsMarginEnabled(DependencyObject obj, bool value) => obj.SetValue(IsMarginEnabledProperty, value);

		public static readonly DependencyProperty IsMarginEnabledProperty =
			DependencyProperty.RegisterAttached("IsMarginEnabled", typeof(bool), typeof(MenuMarginBehavior), new PropertyMetadata(false, OnMarginChanged));

		public static Thickness GetAdditionalMargin(DependencyObject obj) => (Thickness)obj.GetValue(AdditionalMarginProperty);

		public static void SetAdditionalMargin(DependencyObject obj, Thickness value) => obj.SetValue(AdditionalMarginProperty, value);

		public static readonly DependencyProperty AdditionalMarginProperty =
			DependencyProperty.RegisterAttached("AdditionalMargin", typeof(Thickness), typeof(MenuMarginBehavior), new PropertyMetadata(new Thickness(0), OnMarginChanged));

		private static void OnMarginChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var control = (FrameworkElement)sender;
			if (GetIsMarginEnabled(control))
			{
				var additionalMargin = GetAdditionalMargin(control);

				control.Margin = new Thickness(
					additionalMargin.Left,
					additionalMargin.Top,
					additionalMargin.Right,
					additionalMargin.Bottom + TabBarHeight);
			}
			else if (args.OldValue is bool wasEnabled && !wasEnabled && !(bool)args.NewValue)
			{
				// Used to remove additional margin for properties that set IsMarginEnabled dynamically
				control.Margin = new Thickness(0, 0, 0, 0);
			}
		}
	}
}
