#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Views;
using Android.Widget;
using Uno.Disposables;
using Uno.Extensions;
using Uno.UI;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ApplicationTemplate.View.Controls
{
	public partial class SwipeRefresh
	{
		private NativeSwipeRefresh _nativeSwipeRefresh;
		private readonly SerialDisposable _refreshSubscription = new SerialDisposable();

		public SwipeRefresh()
		{
			DefaultStyleKey = typeof(SwipeRefresh);
		}

		/// <summary>
		/// The Android progress indicator supports additional colors which will be used when it animates.
		/// </summary>
		public IList<SolidColorBrush> AdditionalIndicatorColors { get; set; } = new List<SolidColorBrush>();

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			_nativeSwipeRefresh = this.FindFirstChild<NativeSwipeRefresh>();

			if (_nativeSwipeRefresh == null)
			{
				throw new NotSupportedException($"SwipeRefresh requires a control of type {nameof(NativeSwipeRefresh)} in its hierarchy.");
			}

			SubscribeToRefresh();
			_nativeSwipeRefresh.Content = Content as Android.Views.View;
			SetIndicatorColors();
			SetIndicatorOffset(IndicatorOffset);
		}

		protected override void OnNativeLoaded()
		{
			base.OnNativeLoaded();
			if (_nativeSwipeRefresh != null)
			{
				SubscribeToRefresh();
			}
		}

		protected override void OnNativeUnloaded()
		{
			base.OnNativeUnloaded();
			_refreshSubscription.Disposable = null;
		}

		private void SubscribeToRefresh()
		{
			_nativeSwipeRefresh.Refresh += OnNativeRefresh;

			_refreshSubscription.Disposable = new ActionDisposable(() => _nativeSwipeRefresh.Refresh -= OnNativeRefresh);
		}

		private void OnNativeRefresh(object sender, EventArgs e)
		{
			OnRefresh();
		}

		protected override void OnContentChanged(object oldValue, object newValue)
		{
			base.OnContentChanged(oldValue, newValue);

			if (_nativeSwipeRefresh != null)
			{
				_nativeSwipeRefresh.Content = Content as Android.Views.View;
			}
		}

		partial void OnIsRefreshingChanged(bool newValue)
		{
			if (_nativeSwipeRefresh != null)
			{
				_nativeSwipeRefresh.Refreshing = newValue;
			}
		}

		partial void OnIndicatorOffsetChanged(Point newValue)
		{
			SetIndicatorOffset(newValue);
		}

		private void SetIndicatorOffset(Point newValue)
		{
			if (newValue.Y != 0)
			{
				//the hardcoded 64 here is the constant height of the indicator
				_nativeSwipeRefresh?.SetProgressViewEndTarget(true, ViewHelper.LogicalToPhysicalPixels(newValue.Y + 64));
			}
		}

		partial void OnIndicatorColorChanged(SolidColorBrush newValue)
		{
			SetIndicatorColors();
		}

		private void SetIndicatorColors()
		{
			_nativeSwipeRefresh?.SetColorSchemeColors(GetIndicatorColors());
		}

		private int[] GetIndicatorColors()
		{
			return new[] { IndicatorColor }.Concat(AdditionalIndicatorColors).Safe().Select(c => (int)((Android.Graphics.Color)c.Color)).ToArray();
		}
	}
}
#endif
