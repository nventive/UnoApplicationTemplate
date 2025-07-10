using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.LocalStorage;

/// <inheritdoc/>
public sealed class FakeCredentialsRepository : ICredentialsRepository
{
	/// <inheritdoc/>
	public Task<Credentials> Read(string resource, string username)
	{
		return Task.FromResult(new Credentials
		{
			Username = username,
			Password = "fake-password-for-" + username,
		});
	}

	/// <inheritdoc/>
	public Task Write(string resource, Credentials credentials)
	{
		return Task.CompletedTask;
	}
}
