namespace ApplicationTemplate.Client;

public sealed partial class DadJokesData
{
	public DadJokesData(DadJokeChildData[] children)
	{
		Children = children;
	}

	public DadJokeChildData[] Children { get; }
}
