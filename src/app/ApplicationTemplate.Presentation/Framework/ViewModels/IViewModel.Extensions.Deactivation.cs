using System;
using System.Collections.Generic;
using System.Text;
using Chinook.DynamicMvvm;
using Chinook.DynamicMvvm.Deactivation;

namespace Chinook.DynamicMvvm;

/// <summary>
/// This class exposes extensions methods on <see cref="IViewModel"/> related to the <see cref="IDeactivatable"/> pattern.
/// </summary>
public static class ChinookViewModelExtensionsForDeactivation
{
	/// <summary>
	/// Adds a subcription that implements <see cref="IDeactivatable"/> to the <see cref="IViewModel"/> disposables.
	/// </summary>
	/// <param name="viewModel">The viewModel.</param>
	/// <param name="subscribe">The subscribe function. It will be invoked every time the subscription reactivates.</param>
	/// <param name="isDeactivated">Flag indicating whether the subscription should activate immediately. This is false by default meaning the subscription automatically activates.</param>
	public static void AddDeactivatableSubscription(this IViewModel viewModel, Func<IDisposable> subscribe, bool isDeactivated = false)
	{
		var deactivatable = new DeactivableSubscription(subscribe);
		viewModel.AddDisposable($"DeactivableSubscription_{Guid.NewGuid()}", deactivatable);
	}

	private sealed class DeactivableSubscription : IDeactivatable, IDisposable
	{
		private readonly Func<IDisposable> _subscribe;

		private IDisposable _subscription;
		private bool _isDisposed;

		public DeactivableSubscription(Func<IDisposable> subscribe, bool isDeactivated = false)
		{
			_subscribe = subscribe;
			IsDeactivated = isDeactivated;
			if (!IsDeactivated)
			{
				_subscription = _subscribe();
			}
		}

		public bool IsDeactivated { get; private set; }

		public void Deactivate()
		{
			if (IsDeactivated || _isDisposed)
			{
				return;
			}

			_subscription?.Dispose();

			IsDeactivated = true;
		}

		public void Reactivate()
		{
			if (!IsDeactivated || _isDisposed)
			{
				return;
			}

			_subscription = _subscribe();

			IsDeactivated = false;
		}

		public void Dispose()
		{
			_isDisposed = true;
			_subscription?.Dispose();
		}
	}
}
