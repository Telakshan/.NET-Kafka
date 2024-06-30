using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.QueryHandler;

public interface IQueryHandler
{
    Task<List<PostEntity>> HandleAsync(FindAllPostsQuery query);
}
