using System.Collections.Generic;
using System.Linq;
using ApplicationTemplate.Client;

namespace ApplicationTemplate.Tests.Business;

public partial class PostServiceShould
{
	// Mocked database
	private PostData[] _mockedPosts = new PostData[]
	{
			new PostData.Builder()
				.WithId(1)
				.WithBody("test body 1")
				.WithTitle("test title 1")
				.WithUserIdentifier(12),

			new PostData.Builder()
				.WithId(2)
				.WithBody("test body 2")
				.WithTitle("test title 2")
				.WithUserIdentifier(5),
	};

	private IEnumerable<PostData> GetMockedPosts()
	{
		return _mockedPosts;
	}

	private PostData GetMockedPost(long givenId)
	{
		return _mockedPosts
			.FirstOrDefault(post => post.Id == givenId);
	}
}
