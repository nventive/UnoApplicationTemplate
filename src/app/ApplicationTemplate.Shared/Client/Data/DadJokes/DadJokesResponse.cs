namespace ApplicationTemplate.Client
{
	public record DadJokesResponse
	{
		public DadJokesResponse(DadJokesData data)
		{
			Data = data;
		}

		public DadJokesData Data { get; }
	}
}
