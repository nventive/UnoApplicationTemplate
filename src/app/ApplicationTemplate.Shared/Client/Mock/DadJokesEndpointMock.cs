using System.Threading;
using System.Threading.Tasks;
using GeneratedSerializers;

namespace ApplicationTemplate.Client
{
	public class DadJokesEndpointMock : BaseMock, IDadJokesEndpoint
	{
		public DadJokesEndpointMock(IObjectSerializer serializer)
			: base(serializer)
		{
		}

		public Task<DadJokesResponse> FetchData(CancellationToken ct, string post)
			=> Task.FromResult(new DadJokesResponse(null));
	}
}
