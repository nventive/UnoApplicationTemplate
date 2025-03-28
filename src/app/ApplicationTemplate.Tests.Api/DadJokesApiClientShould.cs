using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.Business;
using ApplicationTemplate.Tests;

namespace DadJokes;

/// <summary>
/// Tests for <see cref="IDadJokesApiClient"/>.
/// </summary>
public sealed class DadJokesApiClientShould : ApiTestBase
{
	[Theory]
	[InlineData(PostTypes.Hot)]
	[InlineData(PostTypes.New)]
	[InlineData(PostTypes.Rising)]
	public async Task FetchData_ReturnsJokes(PostTypes postType)
	{
		// Arrange
		var apiClient = GetService<IDadJokesApiClient>();
		var ct = CancellationToken.None;
		var redditFilter = postType.ToRedditFilter();

		// Act
		var result = await apiClient.FetchData(ct, redditFilter);

		// Assert
		Assert.NotEmpty(result.Data.Children);
	}
}
