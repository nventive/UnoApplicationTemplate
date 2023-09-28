using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApplicationTemplate.DataAccess;

public record PostData
{
	public PostData(long id, string title, string body, long userIdentifier)
	{
		Id = id;
		Title = title;
		Body = body;
		UserIdentifier = userIdentifier;
	}

	public long Id { get; init; }

	public string Title { get; init; }

	public string Body { get; init; }

	[JsonPropertyName("UserId")]
	public long UserIdentifier { get; init; }

	public bool Exists => Id != 0;

	public override string ToString()
	{
		return $"[Id={Id}, Title={Title}, Body={Body}, UserIdentifier={UserIdentifier}]";
	}
}
