using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.UI.Xaml;
using Uno.Extensions;

namespace ApplicationTemplate.Views.Helpers;

public static class FrameworkElementExtensions
{
	public static IObservable<EventPattern<RoutedEventArgs>> ObserveLoaded(this FrameworkElement source)
	{
		source.Validation().NotNull("source");
		return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
			h => source.Loaded += h,
			h => source.Loaded -= h
		);
	}

	public static IObservable<EventPattern<RoutedEventArgs>> ObserveUnloaded(this FrameworkElement source)
	{
		source.Validation().NotNull("source");
		return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
			h => source.Unloaded += h,
			h => source.Unloaded -= h
		);
	}
}
