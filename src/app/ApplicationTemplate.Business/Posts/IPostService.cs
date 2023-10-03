using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace ApplicationTemplate.Business;

public interface IPostService
{
	/// <summary>
	/// Gets the post of the specified id.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/></param>
	/// <param name="postId">Post id</param>
	/// <returns><see cref="Post"/></returns>
	Task<Post> GetPost(CancellationToken ct, long postId);

	/// <summary>
	/// Gets the list of all posts.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/></param>
	/// <returns>List of all posts</returns>
	Task<ImmutableList<Post>> GetPosts(CancellationToken ct);

	/// <summary>
	/// Creates a post.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/></param>
	/// <param name="post"><see cref="Post"/></param>
	/// <returns>New <see cref="Post"/></returns>
	Task<Post> Create(CancellationToken ct, Post post);

	/// <summary>
	/// Updates a post.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/></param>
	/// <param name="postId">Post id</param>
	/// <param name="post"><see cref="Post"/></param>
	/// <returns>Updated <see cref="Post"/></returns>
	Task<Post> Update(CancellationToken ct, long postId, Post post);

	/// <summary>
	/// Deletes a post.
	/// </summary>
	/// <param name="ct"><see cref="CancellationToken"/></param>
	/// <param name="postId">Post id</param>
	/// <returns><see cref="Task"/></returns>
	Task Delete(CancellationToken ct, long postId);
}
