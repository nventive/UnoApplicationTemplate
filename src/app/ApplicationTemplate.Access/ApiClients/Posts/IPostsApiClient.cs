using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace ApplicationTemplate.DataAccess;

/// <summary>
/// Provides access to the posts API.
/// </summary>
[Headers("Authorization: Bearer")]
public interface IPostsApiClient
{
	/// <summary>
	/// Gets the list of all posts.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>A list of posts.</returns>
	[Get("/posts")]
	Task<PostData[]> GetAll(CancellationToken ct);

	/// <summary>
	/// Gets the post of the specified id.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="postId">The post identifier.</param>
	/// <returns>The post.</returns>
	[Get("/posts/{id}")]
	Task<PostData> Get(CancellationToken ct, [AliasAs("id")] long postId);

	/// <summary>
	/// Creates a post.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="post">The post data.</param>
	/// <returns>A new <see cref="PostData"/>.</returns>
	[Post("/posts")]
	Task<PostData> Create(CancellationToken ct, [Body] PostData post);

	/// <summary>
	/// Updates a post.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="postId">The post identifier.</param>
	/// <param name="post">The updated post data.</param>
	/// <returns>The updated <see cref="PostData"/>.</returns>
	[Put("/posts/{id}")]
	Task<PostData> Update(CancellationToken ct, [AliasAs("id")] long postId, [Body] PostData post);

	/// <summary>
	/// Deletes a post.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="postId">The post identifier.</param>
	[Delete("/posts/{id}")]
	Task Delete(CancellationToken ct, [AliasAs("id")] long postId);
}
