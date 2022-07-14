using Uno;

namespace ApplicationTemplate.Client
{
	[GeneratedImmutable]
	public partial class DadJokesResponse
	{
		public DadJokesResponse(DadJokesData data)
		{
			Data = data;
		}

		public DadJokesData Data { get; }
	}
}
