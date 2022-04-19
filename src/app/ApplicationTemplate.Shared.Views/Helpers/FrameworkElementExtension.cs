﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Uno.Extensions;
using Windows.UI.Xaml;

namespace ApplicationTemplate.Views.Helpers
{
	public static class FrameworkElementExtensions
	{
		public static IObservable<System.Reactive.EventPattern<RoutedEventArgs>> ObserveLoaded(this FrameworkElement source)
		{
			source.Validation().NotNull("source");
			return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
								h => source.Loaded += h,
								h => source.Loaded -= h);
		}

		public static IObservable<System.Reactive.EventPattern<RoutedEventArgs>> ObserveUnloaded(this FrameworkElement source)
		{
			source.Validation().NotNull("source");
			return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
								h => source.Unloaded += h,
								h => source.Unloaded -= h);
		}
	}
}
