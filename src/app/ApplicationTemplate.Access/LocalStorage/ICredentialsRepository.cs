using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.LocalStorage;

/// <summary>
/// Provides access to the local credentials secure storage.
/// </summary>
public interface ICredentialsRepository
{
	/// <summary>
	/// Reads the credentials for a given resource and username from the local secure storage.
	/// </summary>
	/// <param name="resource">The resource to read from.</param>
	/// <param name="username">The username linked to the crendtials.</param>
	/// <returns>The persisted credentials.</returns>
	Task<Credentials> Read(string resource, string username);

	/// <summary>
	/// Writes the credentials for a given resource to the local secure storage.
	/// </summary>
	/// <param name="resource">The resource to read from.</param>
	/// <param name="credentials">The credentials to persist.</param>
	Task Write(string resource, Credentials credentials);
}
