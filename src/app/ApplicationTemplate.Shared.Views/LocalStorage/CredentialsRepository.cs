// src/app/ApplicationTemplate.Shared.Views/LocalStorage/CredentialsRepository.cs
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess.LocalStorage;
using Windows.Security.Credentials;

namespace ApplicationTemplate.DataAccess.LocalStorage;

/// <inheritdoc/>
public sealed class CredentialsRepository : ICredentialsRepository
{
	/// <inheritdoc/>
	public Task<Credentials> Read(string resource, string username)
	{
		var vault = new PasswordVault();

		try
		{
			var credential = vault.Retrieve(resource, username);
			credential.RetrievePassword();

			return Task.FromResult(new Credentials
			{
				Username = credential.UserName,
				Password = credential.Password
			});
		}
		catch
		{
			// Return null or throw exception based on your requirements
			return Task.FromResult<Credentials>(null);
		}
	}

	/// <inheritdoc/>
	public Task Write(string resource, Credentials credentials)
	{
		var vault = new PasswordVault();
		var credential = new PasswordCredential(resource, credentials.Username, credentials.Password);

		vault.Add(credential);

		return Task.CompletedTask;
	}
}
