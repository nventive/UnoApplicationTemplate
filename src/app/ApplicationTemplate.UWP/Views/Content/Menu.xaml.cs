using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ApplicationTemplate.Views.Content
{
	public sealed partial class Menu : AttachableUserControl
	{
		public Menu()
		{
			this.InitializeComponent();

			InitializeSafeArea();
		}

		/// <summary>
		/// This method handles the bottom padding for phones like iPhone X.
		/// </summary>
		private void InitializeSafeArea()
		{
			var full = Windows.UI.Xaml.Window.Current.Bounds;
			var bounds = ApplicationView.GetForCurrentView().VisibleBounds;

			var bottomPadding = full.Bottom - bounds.Bottom;

			if (bottomPadding > 0)
			{
				SafeAreaRow.Height = new GridLength(bottomPadding);
			}

			var totalHeight = MenuRoot.RowDefinitions.Sum(rd => rd.Height.Value);
			CloseTranslateAnimation.To = totalHeight;
			MenuTranslateTransform.Y = totalHeight;
		}
	}
}
