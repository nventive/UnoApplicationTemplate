using System.Collections.Generic;

namespace ApplicationTemplate.Tests.Business
{
	public partial class PostServiceShould
	{
		private IEnumerable<PostData> GetMockedPostData()
		{
			return new PostData[]
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
		}
	}
}
