#if WINDOWS_UWP || __ANDROID__ || __IOS__ || __WASM__
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Uno.Extensions;
using Uno.Logging;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ApplicationTemplate.View.Controls
{
	/// <summary>
	/// Control which implements the 'swipe to refresh' or 'pull to refresh' pattern. Shows a visual and calls a refresh trigger
	/// when the user swipes from the top of the control. The appearance and comportment of the refresh indicator is platform-specific, see 
	/// https://material.google.com/patterns/swipe-to-refresh.html and https://developer.apple.com/ios/human-interface-guidelines/ui-controls/refresh-content-controls/
	/// for more details.
	/// </summary>
	public partial class SwipeRefresh : ContentControl
	{
		/// <summary>
		/// A command to be called when the user triggers a refresh.
		/// </summary>
		public ICommand RefreshCommand
		{
			get { return (ICommand)GetValue(RefreshCommandProperty); }
			set { SetValue(RefreshCommandProperty, value); }
		}

		public static readonly DependencyProperty RefreshCommandProperty =
			DependencyProperty.Register("RefreshCommand", typeof(ICommand), typeof(SwipeRefresh), new PropertyMetadata(null));

		/// <summary>
		/// Whether the control is refreshing. The control sets this to true when the refresh is triggered; setting it to false
		/// instructs the control to hide the refresh progress indicator.
		/// </summary>
		public bool IsRefreshing
		{
			get { return (bool)GetValue(IsRefreshingProperty); }
			set { SetValue(IsRefreshingProperty, value); }
		}

		public static readonly DependencyProperty IsRefreshingProperty =
			DependencyProperty.Register("IsRefreshing",
				typeof(bool),
				typeof(SwipeRefresh),
				new PropertyMetadata(
					false,
					(s, e) => (s as SwipeRefresh).OnIsRefreshingChanged((bool)e.NewValue)
				)
			);

		partial void OnIsRefreshingChanged(bool newValue);

		/// <summary>
		/// A color to apply to the refresh progress indicator.
		/// </summary>
		public SolidColorBrush IndicatorColor
		{
			get { return (SolidColorBrush)GetValue(IndicatorColorProperty); }
			set { SetValue(IndicatorColorProperty, value); }
		}

		public static readonly DependencyProperty IndicatorColorProperty =
			DependencyProperty.Register("IndicatorColor",
				typeof(SolidColorBrush),
				typeof(SwipeRefresh),
				new PropertyMetadata(
					new SolidColorBrush(Colors.Black),
					(s, e) => (s as SwipeRefresh).OnIndicatorColorChanged((SolidColorBrush)e.NewValue)
				)
			);

		partial void OnIndicatorColorChanged(SolidColorBrush newValue);

		/// <summary>
		/// Offset to apply to the refresh indicator, in pixels. Android only support Y value.
		/// </summary>
		public Point IndicatorOffset
		{
			get { return (Point)GetValue(IndicatorOffsetProperty); }
			set { SetValue(IndicatorOffsetProperty, value); }
		}

		public static readonly DependencyProperty IndicatorOffsetProperty =
			DependencyProperty.Register("IndicatorOffset",
				typeof(Point),
				typeof(SwipeRefresh),
				new PropertyMetadata(default(Point),
				(s, e) => (s as SwipeRefresh).OnIndicatorOffsetChanged((Point)e.NewValue)));

		partial void OnIndicatorOffsetChanged(Point newValue);

		/// <summary>
		/// Determines whether or not the attached DependencyObject (such as a ScrollViewer or a ListViewBase) should
		/// be inlcuded in the "lookup" for the SwipeRefresh control when it traverses its tree to find its first
		/// scrollable child. Setting "False" opts-out of this discovery.
		/// </summary>
		/// <returns>True/False value indicating whether the DependencyObject will affect the Pull-To-Refresh behavior</returns>
		/// <remarks>Currently ONLY used in the Android lookup process.</remarks>
		public static bool GetIncludeInSwipeRefresh(DependencyObject obj) => (bool)obj.GetValue(IncludeInSwipeRefreshProperty);

		public static void SetIncludeInSwipeRefresh(DependencyObject obj, bool value) => obj.SetValue(IncludeInSwipeRefreshProperty, value);

		public static readonly DependencyProperty IncludeInSwipeRefreshProperty =
			DependencyProperty.RegisterAttached(
				"IncludeInSwipeRefresh",
				typeof(bool),
				typeof(SwipeRefresh),
				new PropertyMetadata(true));

		private void OnRefresh()
		{
			if (RefreshCommand != null)
			{
				IsRefreshing = true;

				if (RefreshCommand.CanExecute(null))
				{
					RefreshCommand.Execute(null);
				}
			}
			else
			{
				if (this.Log().IsEnabled(LogLevel.Warning))
				{
					this.Log().Warn("No command defined for SwipeRefresh control");
				}
			}
		}
	}
}
#endif
