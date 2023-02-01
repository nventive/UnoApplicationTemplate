namespace ApplicationTemplate.Client;

public sealed partial class DadJokesResponse
{
	public DadJokesResponse(DadJokesData data)
	{
		Data = data;
	}

	public DadJokesData Data { get; }
}
