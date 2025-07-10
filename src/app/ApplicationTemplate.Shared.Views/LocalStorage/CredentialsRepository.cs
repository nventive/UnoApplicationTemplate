using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Windows.Security.Credentials;

namespace ApplicationTemplate.DataAccess.LocalStorage;

/// <inheritdoc/>
public sealed class CredentialsRepository : ICredentialsRepository
{
	private readonly DispatcherQueue _dispatcherQueue;

	public CredentialsRepository(DispatcherQueue dispatcherQueue)
	{
		_dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
	}

	/// <inheritdoc/>
	public async Task<Credentials> Read(string resource, string username)
	{
		return await _dispatcherQueue.EnqueueAsync(() =>
		{
			var passwordVault = new PasswordVault();
			var passwordCredential = passwordVault.Retrieve(resource, username);
			passwordCredential.RetrievePassword();

			return new Credentials
			{
				Username = passwordCredential.UserName,
				Password = passwordCredential.Password,
			};
		});
	}

	/// <inheritdoc/>
	public async Task Write(string resource, Credentials credentials)
	{
		await _dispatcherQueue.EnqueueAsync(() =>
		{
			var passwordVault = new PasswordVault();
			passwordVault.Add(new PasswordCredential(resource, credentials.Username, credentials.Password));
		});
	}
}
