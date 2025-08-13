using MediatR;

namespace BuildingBlocks.Core.Pagination;
public interface IPageQuery<out TResponse> : IPageRequest, IRequest<TResponse>
    where TResponse : class
{ }
