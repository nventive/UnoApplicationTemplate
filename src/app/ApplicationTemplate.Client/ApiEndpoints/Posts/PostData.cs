using System.Text.Json.Serialization;

namespace ApplicationTemplate.Client;

public sealed class PostData
{
	public PostData(long id, string title, string body, long userIdentifier)
	{
		Id = id;
		Title = title;
		Body = body;
		UserIdentifier = userIdentifier;
	}

	public long Id { get; }

	public string Title { get; }

	public string Body { get; }

	[JsonPropertyName("UserId")]
	public long UserIdentifier { get; }

	public bool Exists => Id != 0;

	public override string ToString()
	{
		return $"[Id={Id}, Title={Title}, Body={Body}, UserIdentifier={UserIdentifier}]";
	}
}
