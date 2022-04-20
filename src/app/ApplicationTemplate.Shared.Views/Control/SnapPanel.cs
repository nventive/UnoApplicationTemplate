#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uno.Extensions;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace ApplicationTemplate.View.Controls
{
	public class SnapPanel : Panel, IScrollSnapPointsInfo
	{
		// This panel needs to set its second child as big as the outer ScrollViewer, which
		// can't report to us its size until being measured. Catch 22! The trick is to first report
		// an empty measurement, which lets the ScrollViewer arrange us to its own size, then
		// invalidate measurement (only once!) to redo. This is required because the inner ScrollContentPresenter
		// gets clipped depending on the measured size.
		private Size _parentSize = Size.Empty;

		public SnapPanel()
		{
		}

#region Anchor ATTACHED PROPERTY

		public static SnapAnchor GetAnchor(DependencyObject obj)
		{
			return (SnapAnchor)obj.GetValue(AnchorProperty);
		}

		public static void SetAnchor(DependencyObject obj, SnapAnchor value)
		{
			obj.SetValue(AnchorProperty, value);
		}

		public static readonly DependencyProperty AnchorProperty =
			DependencyProperty.RegisterAttached("Anchor", typeof(SnapAnchor), typeof(SnapPanel), new PropertyMetadata(SnapAnchor.None));

#endregion

#region Orientation PROPERTY

		public Orientation Orientation
		{
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}

		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SnapPanel), new PropertyMetadata(Orientation.Horizontal));

#endregion

#region IScrollSnapPointsInfo INTERFACE

		public bool AreHorizontalSnapPointsRegular
		{
			get { return false; }
		}

		public bool AreVerticalSnapPointsRegular
		{
			get { return false; }
		}

		public event EventHandler<object> HorizontalSnapPointsChanged;
		public event EventHandler<object> VerticalSnapPointsChanged;

		private float _initialSnapPoint;

		public float InitialSnapPoint
		{
			get { return _initialSnapPoint; }
			private set
			{
				if (_initialSnapPoint != value)
				{
					_initialSnapPoint = value;
					this.ChangeScrollViewerView(value);
				}
			}
		}

		private List<float> _snapPoints;

		public List<float> SnapPoints
		{
			get { return _snapPoints; }
			private set
			{
				_snapPoints = value;

				if (this.Orientation == Orientation.Horizontal)
				{
					this.HorizontalSnapPointsChanged?.Invoke(this, null);
				}
				else
				{
					this.VerticalSnapPointsChanged?.Invoke(this, null);
				}
			}
		}
		public IReadOnlyList<float> GetIrregularSnapPoints(Orientation orientation, SnapPointsAlignment alignment)
		{
			// Explicitly casting to IReadOnlyList<float> for non-UWA platforms, since we know it's a ReadOnlyCollection underneath.
			return (orientation == this.Orientation)
				? this.SnapPoints.AsReadOnly() as IReadOnlyList<float>
				: new float[0].ToList().AsReadOnly() as IReadOnlyList<float>;
		}

		public float GetRegularSnapPoints(Orientation orientation, SnapPointsAlignment alignment, out float offset)
		{
			throw new NotSupportedException();
		}

#endregion

#region IsInitiallySnapped ATTACHED PROPERTY

		public static bool GetIsInitiallySnapped(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsInitiallySnappedProperty);
		}

		public static void SetIsInitiallySnapped(DependencyObject obj, bool value)
		{
			obj.SetValue(IsInitiallySnappedProperty, value);
		}

		public static readonly DependencyProperty IsInitiallySnappedProperty =
			DependencyProperty.RegisterAttached("IsInitiallySnapped", typeof(bool), typeof(SnapPanel), new PropertyMetadata(false));

#endregion

#region RelativeSize ATTACHED PROPERTY

		public static GridLength GetRelativeSize(DependencyObject obj)
		{
			return (GridLength)obj.GetValue(RelativeSizeProperty);
		}

		public static void SetRelativeSize(DependencyObject obj, GridLength value)
		{
			obj.SetValue(RelativeSizeProperty, value);
		}

		public static readonly DependencyProperty RelativeSizeProperty =
			DependencyProperty.RegisterAttached("RelativeSize", typeof(GridLength), typeof(SnapPanel), new PropertyMetadata(new GridLength(0, GridUnitType.Auto)));

#endregion

#region CalculatedSize ATTACHED PROPERTY (private)

		private static Size GetCalculatedSize(DependencyObject obj)
		{
			return (Size)obj.GetValue(CalculatedSizeProperty);
		}

		private static void SetCalculatedSize(DependencyObject obj, Size value)
		{
			obj.SetValue(CalculatedSizeProperty, value);
		}

		public static readonly DependencyProperty CalculatedSizeProperty =
			DependencyProperty.RegisterAttached("CalculatedSize", typeof(Size), typeof(SnapPanel), new PropertyMetadata(Size.Empty));

#endregion

#region SnapPointInfo ATTACHED PROPERTY

		public static SnapPointInfo GetSnapPointInfo(DependencyObject obj)
		{
			return (SnapPointInfo)obj.GetValue(SnapPointInfoProperty);
		}

		private static void SetSnapPointInfo(DependencyObject obj, SnapPointInfo value)
		{
			obj.SetValue(SnapPointInfoProperty, value);
		}

		public static readonly DependencyProperty SnapPointInfoProperty =
			DependencyProperty.RegisterAttached("SnapPointInfo", typeof(SnapPointInfo), typeof(SnapPanel), new PropertyMetadata(null));

#endregion

		protected override Size MeasureOverride(Size availableSize)
		{
			if (_parentSize.IsEmpty)
			{
				// Don't waste time measuring 
				return new Size(0, 0);
			}

			var totalSize = new Size(0, 0);
			var snapPoints = new List<float>();
			var parentValue = _parentSize.GetOrientedValue(this.Orientation);

			foreach (var child in this.Children)
			{
				var relativeSize = GetRelativeSize(child);
				var actualSize = Size.Empty;

				if ((relativeSize == null) || (relativeSize.IsAuto))
				{
					child.Measure(availableSize);
					actualSize = child.DesiredSize;
				}
				else
				{
					actualSize = availableSize.ReplaceOrientedValue(relativeSize.IsAbsolute ? relativeSize.Value : relativeSize.Value * parentValue, this.Orientation);
					child.Measure(actualSize);

					// Making sure that we don't return an infinite size. We calculate it for the orientation of the panel, 
					// but the other orientation may not be limited.  
					var otherOrientation = GetOpposite(Orientation);

					var otherSize = actualSize.GetOrientedValue(otherOrientation);

					actualSize = actualSize.ReplaceOrientedValue(
						Math.Max(double.IsInfinity(otherSize) ? 0 : otherSize, child.DesiredSize.GetOrientedValue(otherOrientation)),
						otherOrientation
					);

					// That child might want less, but we'll give it all this anyway. 
					// Let's avoid calculations on the arrange pass.
					SetCalculatedSize(child, actualSize);
				}

				var childValue = actualSize.GetOrientedValue(this.Orientation);

				// As a courtesy to parent controls, we attach all possible snap points.
				var info = new SnapPointInfo(
					(float)totalSize.GetOrientedValue(this.Orientation),
					(float)(totalSize.GetOrientedValue(this.Orientation) + (childValue - parentValue) / 2),
					(float)(totalSize.GetOrientedValue(this.Orientation) + childValue - parentValue));

				SetSnapPointInfo(child, info);

				var anchor = GetAnchor(child);

				if ((anchor & SnapAnchor.Near) == SnapAnchor.Near)
				{
					snapPoints.Add(info.NearSnapPoint);
				}

				if ((anchor & SnapAnchor.Center) == SnapAnchor.Center)
				{
					snapPoints.Add(info.CenterSnapPoint);
				}

				if ((anchor & SnapAnchor.Far) == SnapAnchor.Far)
				{
					snapPoints.Add(info.FarSnapPoint);
				}

				totalSize = totalSize.AddOriented(actualSize, this.Orientation);
				availableSize = availableSize.SubtractOriented(actualSize, this.Orientation);
			}

			// We can ignore duplicates.
			snapPoints.Sort();
			this.SnapPoints = snapPoints;

			return totalSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (_parentSize.IsEmpty)
			{
				// Thanks for this first measurement pass! We now need a second...
				_parentSize = finalSize;
				this.InvalidateMeasure();

				return finalSize;
			}

			// Every item gets stacked normally like a StackPanel.
			var totalSize = new Size(0, 0);
			float? initialOffset = null;

			foreach (var child in this.Children)
			{
				var actualSize = GetCalculatedSize(child);

				if (actualSize.IsEmpty)
				{
					actualSize = child.DesiredSize;
				}

				var isHorizontal = (this.Orientation == Orientation.Horizontal);

				var rect = actualSize.GetOrientedRect(totalSize, this.Orientation);
				var element = child as FrameworkElement;

				if (element != null)
				{
					rect = actualSize.GetFinalRect(
						isHorizontal ? totalSize.Width : 0,
						isHorizontal ? 0 : totalSize.Height,
						finalSize.SubtractOriented(totalSize, this.Orientation),
						isHorizontal ? null as HorizontalAlignment? : element.HorizontalAlignment,
						isHorizontal ? element.VerticalAlignment : null as VerticalAlignment?);
				}

				child.Arrange(rect);

				if (SnapPanel.GetIsInitiallySnapped(child) && !initialOffset.HasValue)
				{
					initialOffset = (float)((this.Orientation == Orientation.Horizontal) ? rect.X : rect.Y);
				}

				totalSize = totalSize.AddOriented(actualSize, this.Orientation);
			}

			if (initialOffset.HasValue)
			{
				this.InitialSnapPoint = initialOffset.Value;
			}


			return totalSize;
		}

		private void ChangeScrollViewerView(double? offset)
		{
			if (offset.HasValue)
			{
				double? horizontalOffset = (this.Orientation == Orientation.Horizontal) ? offset : null;
				double? verticalOffset = (this.Orientation == Orientation.Vertical) ? offset : null;

				var scrollViewer = this.FindFirstParent<ScrollViewer>(false);

				if (scrollViewer != null)
				{
					scrollViewer.ChangeView(horizontalOffset, verticalOffset, null, disableAnimation: true);
				}
			}
		}

		public static Orientation GetOpposite(Orientation orientation)
		{
			return (orientation == Orientation.Horizontal)
				? Orientation.Vertical
				: Orientation.Horizontal;
		}
	}
}
#endif
