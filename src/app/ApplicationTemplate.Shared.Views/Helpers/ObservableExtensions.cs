﻿using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Uno;
#if WINDOWS_UWP || __WASM__
using Windows.UI.Xaml;
#elif __ANDROID__ || __IOS__
using Windows.UI.Xaml;
using FrameworkElement = Windows.UI.Xaml.FrameworkElement;
#endif

namespace ApplicationTemplate.Views.Helpers
{
	[Flags]
	internal enum SubscribeToElementOptions : byte
	{
		None = 0,

		/// <summary>
		/// Default is StartLoaded | ResubscribeOnLoadEvenIfCompleted
		/// </summary>
		Default = StartLoaded | ResubscribeOnLoadEvenIfCompleted,

		/// <summary>
		/// Consider the control as loaded on subscribe
		/// </summary>
		StartLoaded = 1,

		/// <summary>
		/// If observable completes or fails, re-subscribe on next loaded
		/// </summary>
		ResubscribeOnLoadEvenIfCompleted = 2,

		/// <summary>
		/// On subscribe, try to determine if the control is loaded or not (Check if Parent is null, which is only a partial solution!).
		/// <remarks>This overrides the <see cref="StartLoaded"/>.</remarks>
		/// </summary>
		DetectIsLoaded = 4
	}

	[Flags]
	internal enum UiEventSubscriptionsOptions
	{
		/// <summary>
		/// Default is ImmediateSubscribe
		/// </summary>
		Default = ImmediateSubscribe,

		/// <summary>
		/// Subscribe and Unsubscribe will be enforced on Dispacther scheduler
		/// </summary>
		/// <remarks>Be sure to not miss an event between subscribe to observable and real event handler add</remarks>
		DispatcherOnly = 0,

		/// <summary>
		/// Add event handler immediatly on Subscribe.
		/// </summary>
		/// <remarks>This mean you must call subscribe on dispatcher</remarks>
		ImmediateSubscribe = 1,

		/// <summary>
		/// Remove event handler immediatly on Dispose / Complete.
		/// </summary>
		/// <remarks>This mean you must dispose subscription on dispatcher</remarks>
		ImmediateUnsubscribe = 2
	}

	internal static class ObservableExtensions
	{
		/// <summary>
		/// Subscribe to the observable sequence when the <paramref name="element"/> is loaded, and automatically unsubscribe when it's unloaded.
		/// </summary>
		public static IDisposable SubscribeToElement<T>(
			this IObservable<T> source,
			FrameworkElement element,
			Action<T> onNext,
			Action<Exception> onError,
			Action onCompleted = null,
			SubscribeToElementOptions options = SubscribeToElementOptions.Default)
		{
			return SubscribeToElement(source, element, Observer.Create(onNext, onError, onCompleted ?? Actions.Null), options);
		}

		/// <summary>
		/// Subscribe to the observable sequence when the <paramref name="element"/> is loaded, and automatically unsubscribe when it's unloaded.
		/// </summary>
		public static IDisposable SubscribeToElement<T>(this IObservable<T> source, FrameworkElement element, IObserver<T> observer, SubscribeToElementOptions options = SubscribeToElementOptions.Default)
		{
			var mustStartLoaded = (options & SubscribeToElementOptions.StartLoaded) == SubscribeToElementOptions.StartLoaded;
			var mustResubscribeOnLoadEvenIfCompleted = (options & SubscribeToElementOptions.ResubscribeOnLoadEvenIfCompleted) == SubscribeToElementOptions.ResubscribeOnLoadEvenIfCompleted;
			var detectIsLoaded = (options & SubscribeToElementOptions.DetectIsLoaded) == SubscribeToElementOptions.DetectIsLoaded;

			var isLoadedObservable = GetIsLoadedObservable(element, mustStartLoaded, detectIsLoaded);

			var toSubscribe = isLoadedObservable
				.DistinctUntilChanged()
				.SelectManyDisposePrevious(
					loaded => loaded
						? source
						: Observable.Empty<T>(Scheduler.Immediate));

			if (!mustResubscribeOnLoadEvenIfCompleted)
			{
				toSubscribe = toSubscribe.TakeUntil(source.Materialize().Where(n => n.Kind == NotificationKind.OnError || n.Kind == NotificationKind.OnCompleted));
			}

			return toSubscribe.Subscribe(observer);
		}

		private static IObservable<bool> GetIsLoadedObservable(FrameworkElement element, bool mustStartLoaded, bool detectIsLoaded)
		{
#if __ANDROID__
            var parent = (element as Android.Views.View).Parent;

			return element.ObserveIsLoaded(detectIsLoaded ? parent != null : mustStartLoaded);
#elif __IOS__
			var parent = (element as UIKit.UIView).Superview;

			return element.ObserveIsLoaded(detectIsLoaded ? parent != null : mustStartLoaded);
#else
			return element.ObserveIsLoaded(detectIsLoaded ? element.Parent != null : mustStartLoaded);
#endif
		}

		private static IObservable<bool> ObserveIsLoaded(this FrameworkElement element, bool? initialValue = null)
		{
			var isLoaded = element
				.ObserveLoaded()
				.Select(_ => true)
				.Merge(element.ObserveUnloaded().Select(_ => false));

			if (initialValue.HasValue)
			{
				isLoaded = isLoaded.StartWith(Scheduler.Immediate, initialValue.Value);
			}

			return isLoaded;
		}

		/// <summary>
		/// Helper to create an observable sequence "FromEventPattern" on a DependencyObject using right scheduler.
		/// </summary>
		public static IObservable<EventPattern<TArgs>> FromEventPattern<THandler, TArgs>(
			Action<THandler> addHandler,
			Action<THandler> removeHandler,
			FrameworkElement element,
			UiEventSubscriptionsOptions options)
		{
			var immediateSubscribe = options.HasFlag(UiEventSubscriptionsOptions.ImmediateSubscribe);
			var immediateUnsubscribe = options.HasFlag(UiEventSubscriptionsOptions.ImmediateUnsubscribe);

			if (immediateSubscribe && immediateUnsubscribe)
			{
				return Observable.FromEventPattern<THandler, TArgs>(addHandler, removeHandler, Scheduler.Immediate);
			}
			else
			{
				var dispatcher = new MainDispatcherScheduler(element.Dispatcher);

				if (immediateSubscribe)
				{
					return Observable.FromEventPattern<THandler, TArgs>(addHandler, h => dispatcher.Schedule(() => removeHandler(h)), Scheduler.Immediate);
				}
				else if (immediateUnsubscribe)
				{
					return Observable.FromEventPattern<THandler, TArgs>(h => dispatcher.Schedule(() => addHandler(h)), removeHandler, Scheduler.Immediate);
				}
				else
				{
					return Observable.FromEventPattern<THandler, TArgs>(addHandler, removeHandler, dispatcher);
				}
			}
		}

		internal static IObservable<EventPattern<RoutedEventArgs>> ObserveLoaded(this FrameworkElement element, UiEventSubscriptionsOptions options = UiEventSubscriptionsOptions.Default)
		{
			return FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				h => element.Loaded += h,
				h => element.Loaded -= h,
				element,
				options
			);
		}

		private static IObservable<EventPattern<RoutedEventArgs>> ObserveUnloaded(this FrameworkElement element, UiEventSubscriptionsOptions options = UiEventSubscriptionsOptions.Default)
		{
			return FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				h => element.Unloaded += h,
				h => element.Unloaded -= h,
				element,
				options
			);
		}

		internal static IObservable<TResult> SelectManyDisposePrevious<TSource, TResult>(this IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
		{
			return Observable.Create<TResult>(
				observer =>
				{
					var serialDisposable = new SerialDisposable();
					var gate = new object();
					var isWorking = false; // We are currently observing a child
					var isCompleted = false; // The parent observable has completed

					var disposable = source.Subscribe(
						next =>
						{
							isWorking = true;
							serialDisposable.Disposable = null;
							var projectedSource = selector(next);
							serialDisposable.Disposable = projectedSource
								.Subscribe(
									observer.OnNext,
									observer.OnError,
									() =>
									{
										lock (gate)
										{
											isWorking = false;
											if (isCompleted)
											{
												observer.OnCompleted();
											}
										}
									}
								);
						},
						observer.OnError,
						() =>
						{
							lock (gate)
							{
								isCompleted = true;
								if (!isWorking)
								{
									observer.OnCompleted();
								}
							}
						});

					return new CompositeDisposable(disposable, serialDisposable).Dispose;
				});
		}
	}
}

