using System;

namespace ApplicationTemplate.DataAccess;

public sealed class PostsApiClientException : Exception
{
	public PostsApiClientException()
	{
	}

	public PostsApiClientException(string message)
		: base(message)
	{
	}

	public PostsApiClientException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	public PostsApiClientException(PostErrorResponse errorResponse)
	{
	}
}
