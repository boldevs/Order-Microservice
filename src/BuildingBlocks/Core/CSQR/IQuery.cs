using MediatR;

namespace BuildingBlocks.Core.CSQR;
public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}
