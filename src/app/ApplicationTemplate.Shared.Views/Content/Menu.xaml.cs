using System;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace ApplicationTemplate.Views.Content;

public sealed partial class Menu : AttachableUserControl
{
	public Menu()
	{
		this.InitializeComponent();

		InitializeSafeArea();
	}

	private void InitializeSafeArea()
	{
		var bottomPadding = GetSafeAreaBottomPadding();

		if (bottomPadding > 0)
		{
			SafeAreaRow.Height = new GridLength(bottomPadding);
		}
	}

	private double GetSafeAreaBottomPadding()
	{
#if __MOBILE__
		var bounds = App.Instance!.CurrentWindow!.Bounds;
		var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;
		var safeAreaBottomPadding = bounds.Bottom - visibleBounds.Bottom;
		return safeAreaBottomPadding;
#else
		return 0;
#endif
	}
}
