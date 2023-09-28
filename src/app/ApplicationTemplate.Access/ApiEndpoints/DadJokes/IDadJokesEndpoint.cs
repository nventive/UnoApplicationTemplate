using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace ApplicationTemplate.DataAccess;

public interface IDadJokesEndpoint
{
	/// <summary>
	/// Returns a list of dad jokes based on /r/dadjokes.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/></param>
	/// <param name="typePost"><see cref="string"/></param>
	/// <returns>List of quotes</returns>
	[Get("/{typePost}.json")]
	Task<DadJokesResponse> FetchData(CancellationToken ct, string typePost);
}
