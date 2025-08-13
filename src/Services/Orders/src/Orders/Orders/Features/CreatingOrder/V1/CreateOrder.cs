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

namespace Orders.Orders.Features.CreatingOrder.V1;

public record CreateOrder(
    string OrderNumber,
    string AccountId,
    DateTime OrderDate,
    OrderStatus OrderStatus,
    decimal TotalPrice) : ICommand<CreateOrderResult>, IInternalCommand
{
    public Guid Id { get; } = NewId.NextGuid();
}


// Result
public record CreateOrderResult(Guid Id);

// Domain Event
public record OrderCreatedDomainEvent(
    Guid Id,
    string OrderNumber,
    Guid AccountId,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalPrice) : IDomainEvent;

// Validator
internal class CreateOrderValidator : AbstractValidator<CreateOrder>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderNumber).NotEmpty().WithMessage("Order number is required.");
        RuleFor(x => x.AccountId).NotEmpty().WithMessage("Account ID is required.");
        RuleFor(x => x.OrderDate).NotEmpty().WithMessage("Order date is required.");
        RuleFor(x => x.TotalPrice).GreaterThan(0).WithMessage("Total price must be greater than zero.");
    }
}

// Handler
internal class CreateOrderHandler : ICommandHandler<CreateOrder, CreateOrderResult>
{
    private readonly OrderDbContext _orderDbContext;
    public CreateOrderHandler(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }
    public async Task<CreateOrderResult> Handle(CreateOrder request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderAlreadyException();
        }

        var OrderEntity = Models.Order.Create(
            OrderId.Of(request.Id),
            OrderNumber.Of(request.OrderNumber),
            Accountid.Of(Guid.Parse(request.AccountId)),
            OrderDate.Of(request.OrderDate),
            request.OrderStatus,
            TotalPrice.Of(request.TotalPrice));

        var newOrder = (await _orderDbContext.Orders.AddAsync(OrderEntity, cancellationToken)).Entity;

        return new CreateOrderResult(newOrder.Id);
    }
}

// DTOs
public record OrderCreatedRequestDto(
    Guid Id,
    string OrderNumber,
    Guid AccountId,
    DateTime OrderDate,
    OrderStatus Status,
    decimal TotalPrice);

// Response DTO
public record CreateOrderResponeDto(Guid Id);


// Endpoint 
public class CreateOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost($"{EndpointConfig.BaseApiPath}/order", async (OrderCreatedRequestDto request,
                 IMediator mediator, IMapper mapper,
                 CancellationToken cancellationToken) =>
        {
            var command = mapper.Map<CreateOrder>(request);

            var result = await mediator.Send(command, cancellationToken);

            var response = result.Adapt<CreateOrderResponeDto>();

            return Results.CreatedAtRoute("order", new { id = result.Id }, response);
        })
             //.RequireAuthorization(nameof(ApiScope))
             .WithName("CreateOrder")
             .WithApiVersionSet(builder.NewApiVersionSet("Order").Build())
             .Produces<CreateOrderResponeDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .WithSummary("Create Order")
             .WithDescription("Create Order")
             .WithOpenApi()
             .HasApiVersion(1.0);

        return builder;
    }
}


