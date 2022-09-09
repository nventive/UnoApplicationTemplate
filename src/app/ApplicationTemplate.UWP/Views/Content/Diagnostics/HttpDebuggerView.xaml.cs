using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ApplicationTemplate.Client;
using Nventive.View.Converters;
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

namespace ApplicationTemplate.Views.Content.Diagnostics;

public sealed partial class HttpDebuggerView : UserControl
{
	public HttpDebuggerView()
	{
		this.InitializeComponent();

		var view = ApplicationView.GetForCurrentView();
		TrySetDetailsScrollViewerMaxHeight(view);
		view.VisibleBoundsChanged += OnVisibleBoundsChanged;
	}

	private void OnVisibleBoundsChanged(ApplicationView sender, object args)
	{
		TrySetDetailsScrollViewerMaxHeight(sender);
	}

	private void TrySetDetailsScrollViewerMaxHeight(ApplicationView applicationView)
	{
		var height = applicationView.VisibleBounds.Height;
		if (height > 0)
		{
			DetailsScrollViewer.MaxHeight = height * 0.60;
		}
	}
}

public class HttpTraceStatusCustomValueConverter : ConverterBase
{
	public object FallbackValue { get; set; }

	public object ValueWhenPending { get; set; }

	public object ValueWhenReceived { get; set; }

	public object ValueWhenFailed { get; set; }

	public object ValueWhenCancelled { get; set; }

	protected override object Convert(object value, Type targetType, object parameter)
	{
		return value switch
		{
			HttpTraceStatus.Pending => ValueWhenPending,
			HttpTraceStatus.Received => ValueWhenReceived,
			HttpTraceStatus.Failed => ValueWhenFailed,
			HttpTraceStatus.Cancelled => ValueWhenCancelled,
			_ => FallbackValue,
		};
	}
}
