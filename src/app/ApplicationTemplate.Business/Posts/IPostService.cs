using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business;

/// <summary>
/// Provides access to the posts.
/// </summary>
public interface IPostService
{
	/// <summary>
	/// Gets the post of the specified id.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="postId">The post identifier.</param>
	/// <returns>The requested <see cref="Post"/>.</returns>
	Task<Post> GetPost(CancellationToken ct, long postId);

	/// <summary>
	/// Gets the list of all posts.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <returns>A list containing all posts.</returns>
	Task<ImmutableList<Post>> GetPosts(CancellationToken ct);

	/// <summary>
	/// Creates a post.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="post">The <see cref="Post"/> to create.</param>
	/// <returns>The newly created <see cref="Post"/>.</returns>
	Task<Post> Create(CancellationToken ct, Post post);

	/// <summary>
	/// Updates a post.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="postId">The post identifier.</param>
	/// <param name="post">The <see cref="Post"/> to update.</param>
	/// <returns>The updated <see cref="Post"/>.</returns>
	Task<Post> Update(CancellationToken ct, long postId, Post post);

	/// <summary>
	/// Deletes a post.
	/// </summary>
	/// <param name="ct">The <see cref="CancellationToken"/>.</param>
	/// <param name="postId">The post identifier.</param>
	Task Delete(CancellationToken ct, long postId);
}
