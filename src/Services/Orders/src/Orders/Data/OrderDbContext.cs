using BuildingBlocks.EFCore;
using BuildingBlocks.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Orders.Data;
public sealed class OrderDbContext : AppDbContextBase
{
    public OrderDbContext(DbContextOptions options,
        ICurrentUserProvider? currentUserProvider = null,
        ILogger<AppDbContextBase>? logger = null) : base(options, currentUserProvider, logger)
    {
    }

    public DbSet<Orders.Models.Order> Orders  => Set<Orders.Models.Order>();
    public DbSet<OrderItems.Models.OrderItem> OrderItems => Set<OrderItems.Models.OrderItem>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(OrderRoot).Assembly);
        builder.FilterSoftDeletedProperties();
        builder.ToSnakeCaseTables();
    }
}
