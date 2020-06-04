using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Logging;

namespace System.Reactive.Linq
{
	public static class ObservableExtensions
	{
		/// <summary>
		/// This operator is like the throttle operator, but it doesn't wait for the <paramref name="delay"/> for initial values.
		/// </summary>
		/// <typeparam name="T">The type of the observable.</typeparam>
		/// <param name="source"></param>
		/// <param name="delay"></param>
		/// <param name="scheduler"></param>
		[SuppressMessage("nventive.Globalization", "NV2005:NV2005 - Simple cyclomatic complexity​​", Justification = "Imported code")]
		[SuppressMessage("nventive.Reliability", "NV0016:NV0016 - Do not create an async void lambda expression", Justification = "Imported code")]
		public static IObservable<T> ThrottleOrImmediate<T>(this IObservable<T> source, TimeSpan delay, IScheduler scheduler)
		{
			// Throttle behavior:
			// A delay is applied before the notifications even if there's only 1 notification in the whole observable sequence.
			// ---(delay)---(notification 1)---(delay)---(notification 2)---...
			//
			// ThrottleOrImmediate behavior:
			// A delay is applied only if 2 notifications would be too close to each other.
			// ---(notification 1)---(delay)---(notification 2)---(delay)---...

			return Observable.Create<T>((observer, ct) =>
			{
				// Next item cannot be send before that time
				var nextItemTime = default(DateTime);

				return Task.FromResult(source.Subscribe(
					async item =>
					{
						var currentTime = DateTime.Now;
						// If we already reach the next item time
						if (currentTime - nextItemTime >= TimeSpan.Zero)
						{
							// Following item will be send only after the set delay
							nextItemTime = currentTime + delay;
							// send current item with scheduler
							scheduler.Schedule(() => observer.OnNext(item));
						}
						else // There is still time before we can send an item
						{
							// we schedule the time for the following item
							nextItemTime = currentTime + delay;
							try
							{
								await Task.Delay(delay, ct);
							}
							catch (TaskCanceledException)
							{
								return;
							}

							// If next item schedule was changed by another item then we stop here
							if (nextItemTime > currentTime + delay)
							{
								return;
							}
							else
							{
								// Set next possible time for an item and send item with scheduler
								nextItemTime = currentTime + delay;
								scheduler.Schedule(() => observer.OnNext(item));
							}
						}
					},
					exception => typeof(ObservableExtensions).Log().ErrorIfEnabled(() => "Error in ThrottleOrImmediate subscription.")
				));
			});
		}
	}
}
