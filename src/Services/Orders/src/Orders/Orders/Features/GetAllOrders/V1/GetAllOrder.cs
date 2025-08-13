using Ardalis.GuardClauses;
using BuildingBlocks.Caching;
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

public record GetAllOrder : IQuery<GetAllOrderResult>, ICacheRequest
{
    public string CacheKey => "GetAvailableFlights";
    public DateTime? AbsoluteExpirationRelativeToNow => DateTime.Now.AddHours(1);
}

// Result
public record GetAllOrderResult(IEnumerable<OrderDto> orderDtos);

// Result
public record GetAllOrderResponseDto(IEnumerable<OrderDto> orderDtos);

// Handler

internal class GetAllOrderHandler : IQueryHandler<GetAllOrder, GetAllOrderResult>
{
    private readonly IMapper _mapper;
    private readonly OrderDbContext _orderDbContext;
    public GetAllOrderHandler(IMapper mapper, OrderDbContext orderDbContext)
    {
        _mapper = mapper;
        _orderDbContext = orderDbContext;
    }
    public async Task<GetAllOrderResult> Handle(GetAllOrder request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var order = (await _orderDbContext.Orders.AsQueryable().ToListAsync(cancellationToken));

        if (order is not null)
        {
            throw new OrderNotFountsException();
        }

        var orderDtos = _mapper.Map<IEnumerable<OrderDto>>(order);

        return new GetAllOrderResult(orderDtos);
    }
}

// Endpoint Config
public class GetAllOrderEndpoint : IMinimalEndpoint
{
    public IEndpointRouteBuilder MapEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet($"{EndpointConfig.BaseApiPath}/flight/get-available-flights",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(new GetAllOrder(), cancellationToken);

                    var response = result.Adapt<GetAllOrderResponseDto>();

                    return Results.Ok(response);
                })
            .RequireAuthorization(nameof(ApiScope))
            .WithName("GetAllOrder")
            .WithApiVersionSet(builder.NewApiVersionSet("Order").Build())
            .Produces<GetAllOrderResponseDto>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get All Order")
            .WithDescription("Get All Order")
            .WithOpenApi()
            .HasApiVersion(1.0);

        return builder;
    }
}


