using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.QueryHandler;

public class QueryHandler : IQueryHandler
{
    private readonly IPostRepository _postRepository;

    public QueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query)
    {
        return await _postRepository.ListAllAsync();
    }
}
