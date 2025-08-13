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
using Orders.OrderItems.ValueObjects;
using Orders.Orders.Enums;
using Orders.Orders.Exceptions;
using Orders.Orders.ValueObjects;

namespace Orders.Orders.Features.UpdateOrder.V1;
public record UpdatedOrder(
    string OrderNumber,
    string AccountId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    decimal TotalPrice) : ICommand<UpdatedOrderResult>, IInternalCommand
{
    public Guid Id { get; } = NewId.NextGuid();
}

// Result
public record UpdatedOrderResult(Guid Id);

// Domain Event
public record OrderUpdatedDomainEvent(
    Guid Id,
    string OrderNumber,
    Guid AccountId,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalPrice) : IDomainEvent;

// Validator
internal class UpdatedOrderValidator : AbstractValidator<UpdatedOrder>
{
    public UpdatedOrderValidator()
    {
        RuleFor(x => x.OrderNumber).NotEmpty().WithMessage("Order number is required.");
        RuleFor(x => x.AccountId).NotEmpty().WithMessage("Account ID is required.");
        RuleFor(x => x.OrderDate).NotEmpty().WithMessage("Order date is required.");
        RuleFor(x => x.TotalPrice).GreaterThan(0).WithMessage("Total price must be greater than zero.");
    }
}

// Handler
internal class UpdatedOrderHandler : ICommandHandler<UpdatedOrder, UpdatedOrderResult>
{
    private readonly OrderDbContext _orderDbContext;
    public UpdatedOrderHandler(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }
    public async Task<UpdatedOrderResult> Handle(UpdatedOrder request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderNotFountException(request.Id);
        }

        order.Update(
            OrderId.Of(request.Id),
            OrderNumber.Of(request.OrderNumber),
            Accountid.Of(Guid.Parse(request.AccountId)),
            OrderDate.Of(request.OrderDate),
            request.OrderStatus,
            TotalPrice.Of(request.TotalPrice));

        var orderUpdated = _orderDbContext.Orders.Update(order).Entity;

        return new UpdatedOrderResult(orderUpdated.Id);
    }
}

// DTOs
public record OrderUpdateRequestDto(
    Guid Id,
    string OrderNumber,
    Guid AccountId,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalPrice);

// Response DTO
public record UpdateOrderResponeDto(Guid Id);


// Endpoint 
public class CreateOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPut($"{EndpointConfig.BaseApiPath}/order", async (OrderUpdateRequestDto request,
                 IMediator mediator, IMapper mapper,
                 CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<UpdatedOrder>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<UpdateOrderResponeDto>();

            return Results.CreatedAtRoute("order", new { id = result.Id }, response);
        })
             .RequireAuthorization(nameof(ApiScope))
             .WithName("UpdatedOrder")
             .WithApiVersionSet(builder.NewApiVersionSet("Order").Build())
             .Produces<UpdateOrderResponeDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .WithSummary("Updated Order")
             .WithDescription("Updated Order")
             .WithOpenApi()
             .HasApiVersion(1.0);

        return builder;
    }
}
