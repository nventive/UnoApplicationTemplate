#if __IOS__
using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using UIKit;
using Windows.Foundation;

namespace ApplicationTemplate.View.Controls
{
	public partial class NativeSwipeRefresh : UIRefreshControl
	{
		public CGPoint IndicatorOffset { get; set; }
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			foreach (var view in Subviews)
			{
				var transform = !IndicatorOffset.IsEmpty ?
					CGAffineTransform.MakeTranslation(IndicatorOffset.X, IndicatorOffset.Y) :
					CGAffineTransform.MakeIdentity();
				view.Transform = transform;
			}
		}
	}
}
#endif
