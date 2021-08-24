using System;
using System.Reactive;

namespace ApplicationTemplate.Business
{
	/// <summary>
	/// This interface allows to notify that a user session has expired through an observable sequence.
	/// </summary>
	public interface INotifySessionExpired
	{
		/// <summary>
		/// Gets an observable sequence of <see cref="Unit"/> indicating that the user session has expired.
		/// </summary>
		/// <returns>The observable sequence.</returns>
		IObservable<Unit> ObserveSessionExpired();
	}
}
