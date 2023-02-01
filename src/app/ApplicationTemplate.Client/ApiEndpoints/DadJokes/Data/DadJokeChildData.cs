namespace ApplicationTemplate.Client;

public sealed partial class DadJokeChildData
{
	public DadJokeChildData(DadJokeContentData data)
	{
		Data = data;
	}

	public DadJokeContentData Data { get; }
}
