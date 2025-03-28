namespace ApplicationTemplate.DataAccess;

public sealed class DadJokeContentData
{
	public DadJokeContentData(string id, string title, string selftext, int totalAwardsReceived, string distinguished)
	{
		Id = id;
		Title = title;
		Selftext = selftext;
		TotalAwardsReceived = totalAwardsReceived;
		Distinguished = distinguished;
	}

	public string Id { get; }

	public string Title { get; }

	public string Selftext { get; }

	public int TotalAwardsReceived { get; }

	public string Distinguished { get; }
}
