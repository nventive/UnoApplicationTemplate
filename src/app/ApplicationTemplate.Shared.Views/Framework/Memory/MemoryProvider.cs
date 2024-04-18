//-:cnd:noEmit
#if __WINDOWS__ || __ANDROID__
using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ApplicationTemplate;

public sealed class MemoryProvider : IMemoryProvider
{
	public IObservable<long> ObservePrivateMemorySize()
	{
		return Observable.Interval(TimeSpan.FromSeconds(1))
			.Select(_ => Process.GetCurrentProcess().PrivateMemorySize64);
	}

	public IObservable<long> ObserveManagedMemorySize()
	{
		return Observable.Interval(TimeSpan.FromSeconds(1))
			.Select(_ => GC.GetTotalMemory(forceFullCollection: false));
	}
}
#endif
//+:cnd:noEmit
