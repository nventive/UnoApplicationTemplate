#if __IOS__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Extensions;
using UIKit;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Extensions.Logging;
using Uno.Logging;
using Uno.Disposables;

namespace ApplicationTemplate.View.Controls
{
	public partial class SwipeRefresh
	{
		private NativeSwipeRefresh _refreshControl;
		private readonly SerialDisposable _refreshSubscription = new SerialDisposable();

		public SwipeRefresh()
		{
			DefaultStyleKey = typeof(SwipeRefresh);

			_refreshControl = new NativeSwipeRefresh();

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (_refreshControl.Superview != null)
			{
				_refreshControl.RemoveFromSuperview();
			}

			// Inject the UIRefreshControl into the first scrollable element found in the hierarchy
			var targetScrollView = this.FindFirstChild<UIScrollView>();
			if (targetScrollView != null)
			{
				foreach (var existingRefresh in targetScrollView.Subviews.OfType<NativeSwipeRefresh>())
				{
					// We can get a scroll view that already has a NativeSwipeRefresh due to template reuse. 
					existingRefresh.RemoveFromSuperview();
				}

				targetScrollView.AddSubview(_refreshControl);
				// Setting AlwaysBounceVertical allows the refresh to work even when the scroll view is not scrollable (ie its content
				// fits entirely in its visible bounds)
				targetScrollView.AlwaysBounceVertical = true;
			}
			else if (this.Log().IsEnabled(LogLevel.Warning))
			{
				this.Log().Warn($"No {nameof(UIScrollView)} found to host refresh indicator; swipe to refresh will not be available.");
			}
		}


		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_refreshControl.ValueChanged += OnRefreshControlValueChanged;
			_refreshSubscription.Disposable = new ActionDisposable(() => _refreshControl.ValueChanged -= OnRefreshControlValueChanged);
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			OnIsRefreshingChanged(false);
			_refreshSubscription.Disposable = null;
		}

		private void OnRefreshControlValueChanged(object sender, EventArgs e)
		{
			OnRefresh();
		}

		partial void OnIsRefreshingChanged(bool newValue)
		{
			if (newValue)
			{
				if (!_refreshControl.Refreshing)
				{
					_refreshControl.BeginRefreshing();
				}
			}
			else
			{
				if (_refreshControl.Refreshing)
				{
					_refreshControl.EndRefreshing();
				}
			}
		}

		partial void OnIndicatorColorChanged(SolidColorBrush newValue)
		{
			if (newValue != null)
			{
				_refreshControl.TintColor = newValue.Color;
			}
		}

		partial void OnIndicatorOffsetChanged(Point newValue)
		{
			_refreshControl.IndicatorOffset = newValue;
		}
	}
}
#endif
