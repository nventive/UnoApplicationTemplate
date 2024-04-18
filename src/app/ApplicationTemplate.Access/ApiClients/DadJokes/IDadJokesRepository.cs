using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the dad jokes API.
/// </summary>
public interface IDadJokesRepository
{
	/// <summary>
	/// Returns a list of dad jokes based on /r/dadjokes.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="typePost">The type of post.</param>
	/// <returns>A list of jokes.</returns>
	[Get("/{typePost}.json")]
	Task<DadJokesResponse> FetchData(CancellationToken ct, string typePost);
}
