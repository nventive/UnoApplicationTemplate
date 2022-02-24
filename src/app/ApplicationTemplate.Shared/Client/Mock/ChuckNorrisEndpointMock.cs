using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeneratedSerializers;
using Refit;

namespace ApplicationTemplate.Client
{
	public class DadJokesEndpointMock : BaseMock, IDadJokesEndpoint
	{
		public DadJokesEndpointMock(IObjectSerializer serializer)
			: base(serializer)
		{
		}

		public async Task<DadJokesResponse> FetchData(CancellationToken ct, string typePost)
		{
			await Task.Delay(TimeSpan.FromSeconds(2));

			return await GetTaskFromEmbeddedResource<DadJokesResponse>();
		}
	}
}
