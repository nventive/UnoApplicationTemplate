using System;
using ApplicationTemplate.DataAccess;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Nventive.View.Converters;
using Windows.UI.ViewManagement;

namespace ApplicationTemplate.Views.Content.Diagnostics;

public sealed partial class HttpDebuggerView : UserControl
{
	public HttpDebuggerView()
	{
		this.InitializeComponent();

		Loaded += OnLoaded;
		Unloaded += OnUnloaded;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		var visibleBoundsHeight = 0d;

//-:cnd:noEmit
#if __WINDOWS__
		var currentWindow = App.Instance.CurrentWindow;
		currentWindow.SizeChanged += OnSizeChanged;
		visibleBoundsHeight = currentWindow.Bounds.Height;
#else
		var applicationView = ApplicationView.GetForCurrentView();
		applicationView.VisibleBoundsChanged += OnVisibleBoundsChanged;
		visibleBoundsHeight = applicationView.VisibleBounds.Height;
#endif
//+:cnd:noEmit

		TrySetDetailsScrollViewerMaxHeight(visibleBoundsHeight);
	}

	private void OnUnloaded(object sender, RoutedEventArgs e)
	{
//-:cnd:noEmit
#if __WINDOWS__
		var currentWindow = App.Instance.CurrentWindow;
		currentWindow.SizeChanged -= OnSizeChanged;
#else
		var applicationView = ApplicationView.GetForCurrentView();
		applicationView.VisibleBoundsChanged -= OnVisibleBoundsChanged;
#endif
//+:cnd:noEmit
	}

	private void OnSizeChanged(object sender, WindowSizeChangedEventArgs args)
	{
		TrySetDetailsScrollViewerMaxHeight(args.Size.Height);
	}

	private void OnVisibleBoundsChanged(ApplicationView sender, object args)
	{
		TrySetDetailsScrollViewerMaxHeight(sender.VisibleBounds.Height);
	}

	/// <summary>
	/// Sets the usable <see cref="ScrollViewer"/> height to 60%.
	/// </summary>
	/// <param name="visibleBoundsHeight">Application visible bounds height.</param>
	private void TrySetDetailsScrollViewerMaxHeight(double visibleBoundsHeight)
	{
		if (visibleBoundsHeight > 0)
		{
			DetailsScrollViewer.MaxHeight = visibleBoundsHeight * 0.60;
		}
	}
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Diagnostics related (Must be isolated | Domain driven).")]
public sealed class HttpTraceStatusCustomValueConverter : ConverterBase
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
