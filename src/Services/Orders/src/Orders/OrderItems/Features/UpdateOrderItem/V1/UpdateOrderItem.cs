
using Ardalis.GuardClauses;
using BuildingBlocks.Core.CSQR;
using BuildingBlocks.Core.Event;
using BuildingBlocks.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.OrderItems.Exceptions;
using Orders.OrderItems.ValueObjects;
using Orders.Orders.Features.CreatingOrder.V1;
using Orders.Orders.ValueObjects;

namespace Orders.OrderItems.Features.UpdateOrderItem.V1;
public record UpdateOrderItem(
    Guid orderId,
    Guid productId,
    int quantity,
    decimal unitPrice) : ICommand<UpdateOrderItemResult>, IInternalCommand
{
    public Guid Id { get; } = NewId.NextGuid();
}

// Result
public record UpdateOrderItemResult(Guid Id);

// Domain Event
public record OrderItemUpdatedDomainEvent(
    Guid Id,
    Guid orderId,
    Guid productId,
    int quantity,
    decimal unitPrice) : IDomainEvent;


// Validator
internal class UpdateOrderItemValidator : AbstractValidator<UpdateOrderItem>
{
    public UpdateOrderItemValidator()
    {
        RuleFor(x => x.orderId).NotEmpty().WithMessage("Order Id is required.");
        RuleFor(x => x.productId).NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.quantity).GreaterThan(0).WithMessage("QTY must be greater than zero.");
        RuleFor(x => x.unitPrice).GreaterThan(0).WithMessage("Total price must be greater than zero.");
    }
}

internal class UpdateOrderItemHandler : ICommandHandler<UpdateOrderItem, UpdateOrderItemResult>
{
    private readonly OrderDbContext _orderDbContext;

    public UpdateOrderItemHandler(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task<UpdateOrderItemResult> Handle(UpdateOrderItem request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var orderItem = await _orderDbContext.OrderItems.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (orderItem is not null)
        {
            throw new OrderItemNotFountException(request.Id);
        }

        orderItem.Update(
            OrderItemId.Of(request.Id),
            OrderId.Of(request.orderId),
            ProductId.Of(request.productId),
            Quantitys.Of(request.quantity),
            UnitPrice.Of(request.unitPrice));

        var orderitemUpdated = _orderDbContext.OrderItems.Update(orderItem).Entity;

        return new UpdateOrderItemResult(orderitemUpdated.Id);
    }
}

// DTOs
public record UpdateOrderItemRequestDto(
    Guid Id,
    Guid orderId,
    Guid productId,
    int quantity,
    decimal unitPrice);

// Response DTO
public record UpdateOrderItemResponeDto(Guid Id);


// Endpoint 
public class UpdateOrderItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/orderitem", async (UpdateOrderItemRequestDto request,
                 IMediator mediator, IMapper mapper,
                 CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UpdateOrderItem>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UpdateOrderItemResponeDto>();

            return Results.CreatedAtRoute("orderitem", new { id = result.Id }, response);
        })
             .RequireAuthorization(nameof(ApiScope))
             .WithName("UpdateOrderItem")
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
