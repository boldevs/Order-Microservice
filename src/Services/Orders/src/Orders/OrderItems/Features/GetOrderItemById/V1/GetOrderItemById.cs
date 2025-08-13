using Ardalis.GuardClauses;
using BuildingBlocks.Core.CSQR;
using BuildingBlocks.Web;
using Duende.IdentityServer.EntityFramework.Entities;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.OrderItems.Dto;
using Orders.OrderItems.Exceptions;
using Orders.Orders.Features.CreatingOrder.V1;

namespace Orders.OrderItems.Features.GetOrderItemById.V1;

public record GetOrderItemById(Guid Id) : IQuery<GetOrderItemByIdResult>;

// Result
public record GetOrderItemByIdResult(OrderItemDto orderDtos);

// Result
public record GetOrderItemByIdResponseDto(OrderItemDto orderDtos);

// Validator
public class GetOrderItemByIdValidator : AbstractValidator<GetOrderItemById>
{
    public GetOrderItemByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

// Handler
internal class GetOrderItemByIdHandler : IQueryHandler<GetOrderItemById, GetOrderItemByIdResult>
{
    private readonly IMapper _mapper;
    private readonly OrderDbContext _orderDbContext;
    public GetOrderItemByIdHandler(IMapper mapper, OrderDbContext orderDbContext)
    {
        _mapper = mapper;
        _orderDbContext = orderDbContext;
    }
    public async Task<GetOrderItemByIdResult> Handle(GetOrderItemById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.OrderItems.AsQueryable().SingleOrDefaultAsync(
            x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderItemNotFountException(request.Id);
        }

        var orderDtos = _mapper.Map<OrderItemDto>(order);

        return new GetOrderItemByIdResult(orderDtos);
    }
}


// Endpoint Config
public class GetOrderByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/orderitem/{{id}}",
                async (Guid id, IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetOrderById(id), cancellationToken);

                    var response = result.Adapt<GetOrderItemByIdResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetOrderItemById")
            .WithApiVersionSet(builder.NewApiVersionSet("OrderItem").Build())
            .Produces<GetOrderByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Order Item By Id")
            .WithDescription("Get Order Item By Id")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}