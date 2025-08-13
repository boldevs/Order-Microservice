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
using Orders.OrderItems.Models;
using Orders.OrderItems.ValueObjects;
using Orders.Orders.Exceptions;
using Orders.Orders.Features.CreatingOrder.V1;
using Orders.Orders.ValueObjects;

namespace Orders.OrderItems.Features.CreateOrderItem.V1;
public record CreateOrderItem(
    Guid orderId,
    Guid productId,
     int quantity,
     decimal unitPrice) : ICommand<CreateOrderItemResult>, IInternalCommand
{
    public Guid Id { get; } = NewId.NextGuid();
}

// Result
public record CreateOrderItemResult(Guid Id);

// Domain Event
public record OrderItemCreatedDomainEvent(
    Guid Id,
    Guid orderId,
    Guid productId,
     int quantity,
     decimal unitPrice) : IDomainEvent;


// Validator
internal class CreateOrderItemValidator : AbstractValidator<CreateOrderItem>
{
    public CreateOrderItemValidator()
    {
        RuleFor(x => x.orderId).NotEmpty().WithMessage("Order Id is required.");
        RuleFor(x => x.productId).NotEmpty().WithMessage("Product ID is required.");
        RuleFor(x => x.quantity).GreaterThan(0).WithMessage("QTY must be greater than zero.");
        RuleFor(x => x.unitPrice).GreaterThan(0).WithMessage("Total price must be greater than zero.");
    }
}

internal class CreateOrderItemHandler : ICommandHandler<CreateOrderItem, CreateOrderItemResult>
{
    private readonly OrderDbContext _orderDbContext;

    public CreateOrderItemHandler(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task<CreateOrderItemResult> Handle(CreateOrderItem request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.OrderItems.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderAlreadyException();
        }

        var orderItem = OrderItem.Create(
            OrderItemId.Of(request.Id),
            OrderId.Of(request.orderId),
            ProductId.Of(request.productId),
            Quantitys.Of(request.quantity),
            UnitPrice.Of(request.unitPrice));

        var newOrderItem = (await _orderDbContext.OrderItems.AddAsync(orderItem, cancellationToken)).Entity;

        return new CreateOrderItemResult(newOrderItem.Id);
    }
}

// DTOs
public record CreateOrderItemRequestDto(
    Guid Id,
    Guid orderId,
    Guid productId,
    int quantity,
    decimal unitPrice);

// Response DTO
public record CreateOrderItemResponeDto(Guid Id);


// Endpoint 
public class CreateOrderItemEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/orderitem", async (CreateOrderItemRequestDto request,
                 IMediator mediator, IMapper mapper,
                 CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<CreateOrderItem>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<CreateOrderItemResponeDto>();

            return Results.CreatedAtRoute("orderitem", new { id = result.Id }, response);
        })
             .RequireAuthorization(nameof(ApiScope))
             .WithName("CreateOrderItem")
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
