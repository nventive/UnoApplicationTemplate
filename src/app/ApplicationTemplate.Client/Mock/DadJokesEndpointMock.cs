using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Client
{
	public class DadJokesEndpointMock : BaseMock, IDadJokesEndpoint
	{
		public DadJokesEndpointMock(JsonSerializerOptions serializerOptions)
			: base(serializerOptions)
		{
		}

		public Task<DadJokesResponse> FetchData(CancellationToken ct, string post)
			=> Task.FromResult(new DadJokesResponse(new DadJokesData(Array.Empty<DadJokeChildData>())));
	}
}
