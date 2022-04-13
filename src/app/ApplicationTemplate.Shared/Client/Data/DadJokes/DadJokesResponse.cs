namespace ApplicationTemplate.Client
{
	public class DadJokesResponse
	{
		public DadJokesResponse(DadJokesData data)
		{
			Data = data;
		}

		public DadJokesData Data { get; }
	}
}
