using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using Chinook.DynamicMvvm;
using CollectionTracking;

namespace ApplicationTemplate.Framework.DynamicMvvm
{
	public class ObservableCollectionFromObservableAdapter<T> : IDisposable
	{
		private readonly IViewModel _viewModel;
		private readonly IObservable<IEnumerable<T>> _source;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly IDisposable _subscription;
		private readonly object _referenceListMutex = new object();

		private IEnumerable<T> _referenceList;

		public ObservableCollectionFromObservableAdapter(IViewModel viewModel, IObservable<IEnumerable<T>> source, IEnumerable<T> initialValue)
		{
			Collection = new ObservableCollection<T>(initialValue);
			_referenceList = initialValue.ToList();
			_viewModel = viewModel;
			_source = source;

			ReadOnlyCollection = new ReadOnlyObservableCollection<T>(Collection);
			_subscription = _source.Subscribe(OnNext);
		}

		public ReadOnlyObservableCollection<T> ReadOnlyCollection { get; }

		public ObservableCollection<T> Collection { get; }

		private void OnNext(IEnumerable<T> list)
		{
			var operations = GetOperationsAndUpdateReferenceList(list);

			if (_viewModel.View == null)
			{
				ApplyOperations();
			}
			else
			{
				_ = _viewModel.View.ExecuteOnDispatcher(_cts.Token, ApplyOperations);
			}

			void ApplyOperations()
			{
				Collection.ApplyOperations(operations);
			}
		}

		/// <summary>
		/// Atomically updates <see cref="_referenceList"/> and returns the operations.
		/// </summary>
		/// <remarks>
		/// We need a separate list than <see cref="Collection"/> to compute the operations because <see cref="Collection"/> is modified on a separate thread most of the time.
		/// It's important because when updates happen really fast, comparing with <see cref="Collection"/> (and not <see cref="_referenceList"/>) would end up applying the same operations multiple times, causing inconsistencies.
		/// </remarks>
		/// <param name="list">The new target list.</param>
		/// <returns>The operations to apply on <see cref="Collection"/>.</returns>
		private IEnumerable<CollectionOperation<T>> GetOperationsAndUpdateReferenceList(IEnumerable<T> list)
		{
			lock (_referenceListMutex)
			{
				var operations = _referenceList.GetOperations(list);
				_referenceList = list.ToList();

				return operations;
			}
		}

		public void Dispose()
		{
			_subscription.Dispose();
			_cts.Cancel();
			_cts.Dispose();
		}
	}
}
