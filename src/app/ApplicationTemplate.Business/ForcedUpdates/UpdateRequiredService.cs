using System;
using System.Reactive.Linq;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate.Business;

/// <summary>
/// Implementation of the <see cref="IUpdateRequiredService"/>.
/// </summary>
public sealed class UpdateRequiredService : IUpdateRequiredService, IDisposable
{
	private readonly IDisposable _subscription;

	/// <summary>
	/// Initializes a new instance of the <see cref="UpdateRequiredService"/> class.
	/// </summary>
	/// <param name="minimumVersionReposiory">A repository that contains an observable we can use to update the app.</param>
	public UpdateRequiredService(IMinimumVersionReposiory minimumVersionReposiory)
	{
		_subscription = minimumVersionReposiory.MinimumVersionObservable
			.Subscribe(version =>
			{
				if (version > new Version("1.1.0"))
				{
					UpdateRequired?.Invoke(this, EventArgs.Empty);
				}
			});
	}

	/// <inheritdoc />
	public event EventHandler UpdateRequired;

	/// <inheritdoc />
	public void Dispose()
	{
		_subscription.Dispose();
		UpdateRequired = null;
	}
}
