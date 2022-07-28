using System.Collections.Generic;
using System.Linq;
using ApplicationTemplate.Client;

namespace ApplicationTemplate.Tests.Business
{
	public partial class PostServiceShould
	{
		// Mocked database
		private PostData[] _mockedPosts = new PostData[]
		{
			new PostData(1, title: "test title 1", body: "test body 1", userIdentifier: 12),
			new PostData(2, title: "test title 2", body: "test body 2", userIdentifier: 5),
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
}
