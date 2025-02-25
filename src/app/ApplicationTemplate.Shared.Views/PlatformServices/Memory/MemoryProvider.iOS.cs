//-:cnd:noEmit
#if __IOS__
using System;
using System.Reactive.Linq;

namespace ApplicationTemplate.DataAccess.PlatformServices;

public sealed class MemoryProvider : IMemoryProvider
{
	public IObservable<long> ObservePrivateMemorySize()
	{
		return Observable.Empty<long>();
	}

	public IObservable<long> ObserveManagedMemorySize()
	{
		return Observable.Interval(TimeSpan.FromSeconds(1))
			.Select(_ => GC.GetTotalMemory(forceFullCollection: false));
	}
}
#endif
//+:cnd:noEmit
