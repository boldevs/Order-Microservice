using Ardalis.GuardClauses;
using BuildingBlocks.Core.CSQR;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.OrderItems.Exceptions;
using Orders.Orders.Features.CreatingOrder.V1;

namespace Orders.OrderItems.Features.DeletedOrderItem.V1;

public record DeleteOrderItem(Guid Id) : ICommand<DeleteOrderItemResult>, IInternalCommand;

// Result
public record DeleteOrderItemResult(Guid Id);

// Domain Event
public record OrderItemDeletedDomainEvent(
    Guid Id,
    Guid orderId,
    Guid productId,
    int quantity,
    decimal unitPrice) : IDomainEvent;


// Validator
internal class DeleteOrderItemValidator : AbstractValidator<DeleteOrderItem>
{
    public DeleteOrderItemValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Order Item Id is required.");
    }
}

internal class DeleteOrderItemHandler : ICommandHandler<DeleteOrderItem, DeleteOrderItemResult>
{
    private readonly OrderDbContext _orderDbContext;

    public DeleteOrderItemHandler(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task<DeleteOrderItemResult> Handle(DeleteOrderItem request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.OrderItems.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderItemNotFountException(request.Id);
        }

        order.Delete(
            order.Id,
            order.OrderId,
            order.ProductId,
            order.Quantity,
            order.UnitPrice);

        var deletedOrderItem = _orderDbContext.OrderItems.Update(order).Entity;

        return new DeleteOrderItemResult(deletedOrderItem.Id);
    }
}

// DTOs
public record DeleteOrderItemRequestDto(
    Guid Id,
    Guid orderId,
    Guid productId,
    int quantity,
    decimal unitPrice);

// Response DTO
public record DeleteOrderItemResponeDto(Guid Id);


// Endpoint 
public class DeleteOrderItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete($"{EndpointConfig.BaseApiPath}/orderitem/{{id}}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(new DeleteOrderItem(id), cancellationToken);

                    return Results.NoContent();
                })
             .RequireAuthorization(nameof(ApiScope))
             .WithName("DeleteOrderItem")
             .WithApiVersionSet(builder.NewApiVersionSet("OrderItem").Build())
             .Produces<CreateOrderResponeDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .WithSummary("Create Order Item")
             .WithDescription("Create Order Item")
             .WithOpenApi()
             .HasApiVersion(1.0);

        return builder;
    }
}
