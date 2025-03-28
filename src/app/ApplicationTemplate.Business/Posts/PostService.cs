using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApplicationTemplate.DataAccess;

namespace ApplicationTemplate.Business;

public partial class PostService : IPostService
{
	private readonly IPostsApiClient _postsRepository;

	public PostService(IPostsApiClient postsRepository)
	{
		_postsRepository = postsRepository;
	}

	public async Task<Post> GetPost(CancellationToken ct, long postId)
	{
		return Post.FromData(await _postsRepository.Get(ct, postId));
	}

	public async Task<ImmutableList<Post>> GetPosts(CancellationToken ct)
	{
		var posts = await _postsRepository.GetAll(ct);

		return posts.Select(data => Post.FromData(data)).ToImmutableList();
	}

	public async Task<Post> Create(CancellationToken ct, Post post)
	{
		return Post.FromData(await _postsRepository.Create(ct, post.ToData()));
	}

	public async Task<Post> Update(CancellationToken ct, long postId, Post post)
	{
		return Post.FromData(await _postsRepository.Update(ct, postId, post.ToData()));
	}

	public async Task Delete(CancellationToken ct, long postId)
	{
		await _postsRepository.Delete(ct, postId);
	}
}
