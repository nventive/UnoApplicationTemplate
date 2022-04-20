#if WINDOWS_UWP
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ApplicationTemplate.View.Controls
{
	[TemplatePart(Name = ScrollViewerPartName, Type = typeof(ScrollViewer))]
	[TemplatePart(Name = SnapPanelPartName, Type = typeof(SnapPanel))]
	[TemplatePart(Name = RefreshContainerPartName, Type = typeof(FrameworkElement))]
	[TemplateVisualState(GroupName = SwipeVisualStates, Name = ReleasedVisualState)]
	[TemplateVisualState(GroupName = SwipeVisualStates, Name = SwipingVisualState)]
	[TemplateVisualState(GroupName = SwipeVisualStates, Name = SwipedVisualState)]
	public partial class SwipeRefresh
	{
		private const string SnapPanelPartName = "PART_SnapPanel";
		private const string ScrollViewerPartName = "PART_ScrollViewer";
		private const string RefreshContainerPartName = "PART_RefreshContainer";

		private const string SwipeVisualStates = "Swipe";
		private const string SwipingVisualState = "Swiping";
		private const string SwipedVisualState = "Swiped";
		private const string ReleasedVisualState = "Released";

		private ScrollViewer _scrollViewer;
		private SnapPanel _snapPanel;
		private FrameworkElement _refreshContainer;

		public SwipeRefresh()
		{
			DefaultStyleKey = typeof(SwipeRefresh);
		}

		/// <summary>
		/// Gets or sets the offset before considering the content swiped. The default value is 100.
		///
		/// </summary>
		/// <remarks>The view cannot bind to this property. It must be set once.</remarks>
		public double SwipedSize { get; set; } = 100D;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			_scrollViewer = this.GetTemplateChild(ScrollViewerPartName) as ScrollViewer;
			if (_scrollViewer == null)
			{
				throw new NotSupportedException("SwipeRefresh requires a ScrollViewer element named " + ScrollViewerPartName + ".");
			}
			_snapPanel = this.GetTemplateChild(SnapPanelPartName) as SnapPanel;
			if (_scrollViewer == null)
			{
				throw new NotSupportedException("SwipeRefresh requires a SnapPanel element named " + SnapPanelPartName + ".");
			}
			_refreshContainer = this.GetTemplateChild(RefreshContainerPartName) as FrameworkElement;
			if (_scrollViewer == null)
			{
				throw new NotSupportedException("SwipeRefresh requires a container named " + RefreshContainerPartName + ".");
			}

			_scrollViewer.ViewChanging += OnScrollViewerViewChanging;
		}

		private void OnScrollViewerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
		{
			if (e.NextView.VerticalOffset == 0)
			{
				this.OnSwiped();
			}
			else if (e.NextView.VerticalOffset < this.SwipedSize)
			{
				this.OnSwiping();
			}
			else if (e.NextView.VerticalOffset >= this.SwipedSize)
			{
				this.OnReleased();
			}
		}

		private void OnSwiping()
		{
			this.GoToState(SwipingVisualState);
		}

		private void OnSwiped()
		{
			this.GoToState(SwipedVisualState);

			if (!this.IsRefreshing)
			{
				// Snap to top of refresh container
				SnapPanel.SetAnchor(_refreshContainer, SnapAnchor.Near);
				_snapPanel.InvalidateMeasure();

				this.IsRefreshing = true;

				if (RefreshCommand?.CanExecute(null) ?? false)
				{
					RefreshCommand.Execute(null);
				}
			}
		}

		private void OnReleased()
		{
			this.GoToState(ReleasedVisualState);
		}

		partial void OnIsRefreshingChanged(bool newValue)
		{
			if (!newValue)
			{
				// Snap back to content
				SnapPanel.SetAnchor(_refreshContainer, SnapAnchor.None);
				_snapPanel.InvalidateMeasure();

				_scrollViewer.ChangeView(null, _snapPanel.InitialSnapPoint, null, disableAnimation: false);
			}
		}

		private void GoToState(string stateName)
		{
			VisualStateManager.GoToState(this, stateName, true);
		}
	}
}
#endif
