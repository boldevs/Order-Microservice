
using Mapster;
using Orders.OrderItems.Dto;
using Orders.Orders.Dtos;
using Orders.Orders.Features.CreatingOrder.V1;

namespace Orders.Orders.Features
{
    public class OrderMappings : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

            config.NewConfig<Models.Order, OrderDto>()
               .Map(d => d.Id, s => s.Id)
               .Map(d => d.OrderNumber, s => s.OrderNumber)
               .Map(d => d.AccountId, s => s.AccountId)
               .Map(d => d.OrderDate, s => s.OrderDate)
               .Map(d => d.Status, s => s.Status)
               .Map(d => d.TotalPrice, s => s.TotalPrice)
               .Map(d => d.OrderItems, s => s.OrderItems.Adapt<List<OrderItemDto>>());

            // 2) IEnumerable<Order> -> GetAllOrderResponseDto
            config.NewConfig<IEnumerable<Models.Order>, GetAllOrderResponseDto>()
                  .ConstructUsing(src => new GetAllOrderResponseDto(src.Adapt<IEnumerable<OrderDto>>()));
                  
            config.NewConfig<OrderCreatedRequestDto, CreateOrder>()
                .MapWith(s => new CreateOrder(
                    s.OrderNumber,
                    s.AccountId.ToString(), // Guid → string
                    s.OrderDate,
                    s.Status,               // same enum type
                    s.TotalPrice
                ));


        }
    }
}