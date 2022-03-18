using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;
using ApplicationTemplate.Framework.DynamicMvvm;

namespace Chinook.DynamicMvvm
{
	public static class ViewModelExtensions2
	{
		public static ObservableCollection<T> GetObservableCollectionFromObservable<T>(this IViewModel viewModel, IObservable<IEnumerable<T>> source, IEnumerable<T> initialValue, [CallerMemberName] string name = null)
		{
			if (viewModel.IsDisposed)
			{
				return null;
			}

			var adapter = viewModel.GetOrCreateDisposable(() => new ObservableCollectionFromObservableAdapter<T>(viewModel, source, initialValue), name);
			return adapter.Collection;
		}

		public static TDisposable GetOrCreateDisposable<TDisposable>(this IViewModel vm, Func<TDisposable> create, [CallerMemberName] string key = null)
			where TDisposable : IDisposable
		{
			if (vm is null)
			{
				throw new ArgumentNullException(nameof(vm));
			}

			if (create is null)
			{
				throw new ArgumentNullException(nameof(create));
			}

			if (vm.TryGetDisposable(key, out var existingDisposable))
			{
				return (TDisposable)existingDisposable;
			}
			else
			{
				var disposable = create();
				vm.AddDisposable(key, disposable);
				return disposable;
			}
		}
	}
}
