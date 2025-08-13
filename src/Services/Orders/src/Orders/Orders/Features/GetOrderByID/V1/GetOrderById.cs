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
using Orders.Orders.Dtos;
using Orders.Orders.Exceptions;

namespace Orders.Orders.Features.CreatingOrder.V1;

public record GetOrderById(Guid Id) : IQuery<GetOrderByIdResult>;

// Result
public record GetOrderByIdResult(OrderDto orderDtos);

// Result
public record GetOrderByIdResponseDto(OrderDto orderDtos);

// Validator
public class GetOrderByIdValidator : AbstractValidator<GetOrderById>
{
    public GetOrderByIdValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Id is required!");
    }
}

// Handler
internal class GetOrderByIdHandler : IQueryHandler<GetOrderById, GetOrderByIdResult>
{
    private readonly IMapper _mapper;
    private readonly OrderDbContext _orderDbContext;
    public GetOrderByIdHandler(IMapper mapper, OrderDbContext orderDbContext)
    {
        _mapper = mapper;
        _orderDbContext = orderDbContext;
    }
    public async Task<GetOrderByIdResult> Handle(GetOrderById request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = await _orderDbContext.Orders.AsQueryable().SingleOrDefaultAsync(
            x => x.Id == request.Id, cancellationToken);

        if (order is not null)
        {
            throw new OrderNotFountsException();
        }

        var orderDtos = _mapper.Map<OrderDto>(order);

        return new GetOrderByIdResult(orderDtos);
    }
}

// Endpoint Config
public class GetOrderByIdEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/order/{{id}}",
                async (Guid id, IMediator mediator, IMapper mapper, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetOrderById(id), cancellationToken);

                    var response = result.Adapt<GetOrderByIdResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetOrderById")
            .WithApiVersionSet(builder.NewApiVersionSet("Order").Build())
            .Produces<GetOrderByIdResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Order By Id")
            .WithDescription("Get Order By Id")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}


