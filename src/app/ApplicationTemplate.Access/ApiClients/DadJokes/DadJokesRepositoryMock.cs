using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.DataAccess;

public class DadJokesRepositoryMock : BaseMock, IDadJokesRepository
{
	public DadJokesRepositoryMock(JsonSerializerOptions serializerOptions)
		: base(serializerOptions)
	{
	}

	public Task<DadJokesResponse> FetchData(CancellationToken ct, string typePost)
		=> this.GetTaskFromEmbeddedResource<DadJokesResponse>();
}
