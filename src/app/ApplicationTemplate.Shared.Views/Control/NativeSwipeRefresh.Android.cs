#if __ANDROID__
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Uno.UI;
using Windows.UI.Xaml;
using System.Numerics;
using AndroidX.SwipeRefreshLayout.Widget;
using Android.Views;
using System.Drawing;
using AndroidX.Core.View;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace ApplicationTemplate.View.Controls
{
	public partial class NativeSwipeRefresh : SwipeRefreshLayout
	{
		public NativeSwipeRefresh() : base(ContextHelper.Current) { }

		// Distance in pixels a touch can wander before we think the user is scrolling
		// https://developer.android.com/reference/android/view/ViewConfiguration.html#getScaledTouchSlop()
		private static readonly float _touchSlop = ViewConfiguration.Get(ContextHelper.Current).ScaledTouchSlop;
		private PointF _gestureStart;
		private bool _isSwiping = false;
		private bool _ignoreGesture = false;
		private Android.Views.View _content;

		public Android.Views.View Content
		{
			get { return _content; }
			set
			{
				if (_content != null)
				{
					RemoveView(_content);
				}
				_content = value;
				if (_content != null)
				{
					AddView(_content);
					//_content.Maybe<IDataContextProvider>(b => b.SetInheritedDataContext(DataContext)); //See note above
				}
			}
		}

		private ViewGroup _descendantScrollable;
		private bool _lastOnInterceptTouchEvent;

		public override bool CanChildScrollUp()
		{
			_descendantScrollable = _descendantScrollable ?? GetDescendantScrollable();

			if (_descendantScrollable != null)
			{
#pragma warning disable CS0618 // Type or member is obsolete
				var canScrollUp = ViewCompat.CanScrollVertically(_descendantScrollable, -1);
#pragma warning restore CS0618 // Type or member is obsolete
				return canScrollUp;
			}
			return base.CanChildScrollUp();
		}

		public override bool OnInterceptTouchEvent(MotionEvent e)
		{
			_lastOnInterceptTouchEvent = base.OnInterceptTouchEvent(e);
			switch (e.Action)
			{
				case MotionEventActions.Down:
					_isSwiping = false;
					_ignoreGesture = false;
					_gestureStart = new PointF(e.GetX(), e.GetY());
					break;
				case MotionEventActions.Move:
					return ShouldInterceptMove(e);
				default:
					_isSwiping = false;
					_ignoreGesture = false;
					break;
			}

			return _lastOnInterceptTouchEvent;
		}

		private bool ShouldInterceptMove(MotionEvent e)
		{
			//If the Swipe already started swiping we let it handle the touch until it is completed.
			if (_isSwiping)
			{
				return true;
			}
			else
			{
				Vector2 cumulativeDistance = new Vector2(
												Math.Abs(e.GetX() - _gestureStart.X),
												Math.Abs(e.GetY() - _gestureStart.Y)
											);

				//Here we validate that the touch is not horizontal. 
				//This is to ensure that it will not override horizontal touches for children such as flipviews.
				if (_ignoreGesture || cumulativeDistance.X > _touchSlop)
				{
					_ignoreGesture = true;
					return false;
				}

				if (cumulativeDistance.Y > _touchSlop && // touchSlop is the minimal distance for us to determine it is a scroll
					cumulativeDistance.Y > cumulativeDistance.X && // Check the scroll is vertical
					e.GetY() - _gestureStart.Y > 0 && // Check if the scroll is positive so that we don't intercept the scrolling of the AVP/List
					!CanChildScrollUp()) // Validate if the child can scroll up so that we only allow swipe refresh when the content is scrolled.
				{
					_isSwiping = true;

					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			if (e.Action == MotionEventActions.Up && !_lastOnInterceptTouchEvent)
			{
				// On MotionEventActions.Up, SwipeRefreshLayout.OnTouchEvent checks the current y against mInitialMotionY and sets 
				// refreshing accordingly. However, if OnInterceptTouchEvent hasn't returned true (because user hasn't dragged far 
				// enough to trigger a drag) then mInitialMotionY hasn't been set properly, giving rise to false-positive refreshes. 
				// Hence we don't call base in this case to prevent the false positive.
				return false;
			}

			var baseOnTouchEvent = base.OnTouchEvent(e);
			// If we are handling touch and OnInterceptTouchEvent() did not return true, it means there are no touch handlers in the
			// children. (This can happen if, eg, the content is not scrollable.) Give OnInterceptTouchEvent another chance so that
			// SwipeRefreshLayout can set its isDragged state correctly, otherwise indicator won't appear.
			if (baseOnTouchEvent && !_lastOnInterceptTouchEvent)
			{
				OnInterceptTouchEvent(e);
			}
			return baseOnTouchEvent;
		}

		private ViewGroup GetDescendantScrollable()
		{
			return getScrollablesInChildren(this).FirstOrDefault();

			IEnumerable<ViewGroup> getScrollablesInChildren(ViewGroup parent)
			{
				for (int i = 0; i < parent.ChildCount; i++)
				{
					var child = parent.GetChildAt(i);

					if (child is ViewGroup v)
					{
						// Get the IncludeInSwipeRefresh attached property on the FrameworkElement
						// (False specifies that the element will not be discovered as a descendant scroller)
						var element = (child as FrameworkElement);
						var ignoreSwipe = element != null
							? !SwipeRefresh.GetIncludeInSwipeRefresh(element)
							: false;

						if (ignoreSwipe)
						{
							continue;
						}
						if (v is UnoTwoDScrollView || v is ScrollView || v is AbsListView || v is RecyclerView)
						{
							yield return v;
						}
						foreach (var vInner in getScrollablesInChildren(v))
						{
							yield return vInner;
						}
					}
				}
			}
		}
	}
}
#endif
