namespace ApplicationTemplate.DataAccess;

public partial class DadJokesResponse
{
	public DadJokesResponse(DadJokesData data)
	{
		Data = data;
	}

	public DadJokesData Data { get; }
}
