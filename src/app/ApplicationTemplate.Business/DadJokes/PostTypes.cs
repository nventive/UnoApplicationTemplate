namespace ApplicationTemplate.Business;

public enum PostTypes
{
	New,
	Hot,
	Rising
}

public static class PostTypeExtensions
{
	public static string ToRedditFilter(this PostTypes pt)
	{
		switch (pt)
		{
			case PostTypes.New:
				return "new";

			case PostTypes.Rising:
				return "rising";

			case PostTypes.Hot:
			default:
				return "hot";
		}
	}
}
