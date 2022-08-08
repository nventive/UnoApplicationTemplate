using System;
using System.Collections.Generic;
using System.Text;
using ApplicationTemplate.Client;

namespace ApplicationTemplate.Business;

public record Post
{
	public long Id { get; init; }

	public string Title { get; init; }

	public string Body { get; init; }

	public long UserIdentifier { get; init; }

	public bool Exists => Id != 0;

	public static Post FromData(PostData data)
	{
		if (data is null)
		{
			return default;
		}

		return new Post
		{
			Id = data.Id,
			Title = data.Title,
			Body = data.Body,
			UserIdentifier = data.UserIdentifier
		};
	}

	public override string ToString()
	{
		return $"[Id={Id}, Title={Title}, Body={Body}, UserIdentifier={UserIdentifier}]";
	}
}

public static class PostExtensions
{
	public static PostData­ ToData(this Post post)
	{
		if (post is null)
		{
			return default;
		}

		return new PostData­(post.Id, post.Title, post.Body, post.UserIdentifier);
	}
}
