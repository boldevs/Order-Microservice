using Ardalis.GuardClauses;
using BuildingBlocks.Core.CSQR;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Orders.Enums;
using Orders.Orders.Exceptions;

namespace Orders.Orders.Features.DeleteOrder.V1;

public record DeletedOrder(Guid Id) : ICommand<DeletedOrderResult>, IInternalCommand;

// Result
public record DeletedOrderResult(Guid Id);

// Domain Event
public record OrderDeletedDomainEvent(
    Guid Id,
    string OrderNumber,
    Guid AccountId,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalPrice) : IDomainEvent;

// Validator
internal class OrderDeleteValidator : AbstractValidator<DeletedOrder>
{
    public OrderDeleteValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

// Handler
internal class DeletedOrderHandler : ICommandHandler<DeletedOrder, DeletedOrderResult>
{
    private readonly OrderDbContext _orderDbContext;
    public DeletedOrderHandler(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }
    public async Task<DeletedOrderResult> Handle(DeletedOrder request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderNotFountException(request.Id);
        }

        order.Delete(
            order.Id,
            order.OrderNumber,
            order.AccountId,
            order.OrderDate,
            order.Status,
            order.TotalPrice);

        var deetedOrder = _orderDbContext.Orders.Update(order).Entity;

        return new DeletedOrderResult(deetedOrder.Id);
    }
}


// Extension Endpoint
public class OrderDeleteEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete(
                $"{EndpointConfig.BaseApiPath}/order/{{id}}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new DeletedOrder(id), cancellationToken);

                    return Results.NoContent();
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("DeleteOrder")
            .WithApiVersionSet(builder.NewApiVersionSet("Order").Build())
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Order")
            .WithDescription("Delete Order")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}