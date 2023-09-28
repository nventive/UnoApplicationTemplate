using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate.Business;

public partial class PostService : IPostService
{
	private readonly IPostEndpoint _postEndpoint;

	public PostService(IPostEndpoint postEndpoint)
	{
		_postEndpoint = postEndpoint;
	}

	public async Task<Post> GetPost(CancellationToken ct, long postId)
	{
		return Post.FromData(await _postEndpoint.Get(ct, postId));
	}

	public async Task<ImmutableList<Post>> GetPosts(CancellationToken ct)
	{
		var posts = await _postEndpoint.GetAll(ct);

		return posts.Select(data => Post.FromData(data)).ToImmutableList();
	}

	public async Task<Post> Create(CancellationToken ct, Post post)
	{
		return Post.FromData(await _postEndpoint.Create(ct, post.ToData()));
	}

	public async Task<Post> Update(CancellationToken ct, long postId, Post post)
	{
		return Post.FromData(await _postEndpoint.Update(ct, postId, post.ToData()));
	}

	public async Task Delete(CancellationToken ct, long postId)
	{
		await _postEndpoint.Delete(ct, postId);
	}
}
