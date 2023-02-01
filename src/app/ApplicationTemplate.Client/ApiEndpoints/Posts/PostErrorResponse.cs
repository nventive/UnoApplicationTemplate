namespace ApplicationTemplate.Client;

public sealed class PostErrorResponse
{
	public PostErrorResponse(PostData data, ErrorData error)
	{
		Data = data;
		Error = error;
	}

	public PostData Data { get; }

	public ErrorData Error { get; }
}
