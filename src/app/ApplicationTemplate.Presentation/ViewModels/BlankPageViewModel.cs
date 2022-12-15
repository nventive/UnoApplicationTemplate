using System;
using System.Reactive.Linq;
using Chinook.DynamicMvvm;

namespace ApplicationTemplate.Presentation;

public class BlankPageViewModel : ViewModel
{
	public long Counter => this.GetFromObservable(
		Observable.Timer(
			dueTime: TimeSpan.Zero,
			period: TimeSpan.FromSeconds(1)
		)
	);
}
