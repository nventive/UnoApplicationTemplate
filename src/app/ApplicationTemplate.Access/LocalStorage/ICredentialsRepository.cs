using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess.LocalStorage;

/// <summary>
/// Provides access to the local credentials secure storage.
/// </summary>
public interface ICredentialsRepository
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="resource"></param>
	/// <param name="username"></param>
	/// <returns></returns>
	Task<Credentials> Read(string resource, string username);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="resource"></param>
	/// <param name="credentials"></param>
	Task Write(string resource, Credentials credentials);
}
