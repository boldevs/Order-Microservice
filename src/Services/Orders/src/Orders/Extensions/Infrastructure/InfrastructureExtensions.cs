using BuildingBlocks.Core;
using BuildingBlocks.EFCore;
using BuildingBlocks.HealthCheck;
using BuildingBlocks.Jwt;
using BuildingBlocks.Logging;
using BuildingBlocks.Mapster;
using BuildingBlocks.MassTransit;
using BuildingBlocks.OpenApi;
using BuildingBlocks.OpenTelemetryCollector;
using BuildingBlocks.PersistMessageProcessor;
using BuildingBlocks.ProblemDetails;
using BuildingBlocks.Web;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orders.Data;
using Serilog;
using System.Threading.RateLimiting;

namespace Orders.Extensions.Infrastructure;
public static class InfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var env = builder.Environment;

        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        builder.Services.AddScoped<IEventMapper, OrderEventMapper>();

        //builder.Services.AddScoped<IEventDispatcher, EventDispatcher>();
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.Services.AddCustomMediatR();
        builder.Services.AddProblemDetails();

        var appOptions = builder.Services.GetOptions<AppOptions>(nameof(AppOptions));
        Console.WriteLine(appOptions.Name);

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 10,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });

        builder.AddCustomDbContext<OrderDbContext>();
        //builder.Services.AddPersistMessageProcessor();

        builder.Services.AddEndpointsApiExplorer();
        builder.AddCustomSerilog(env);
        builder.Services.AddJwt();
        builder.Services.AddAspnetOpenApi();
        builder.Services.AddCustomVersioning();
        builder.Services.AddValidatorsFromAssembly(typeof(OrderRoot).Assembly);
        builder.Services.AddCustomMapster(typeof(OrderRoot).Assembly);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddCustomMassTransit(env, TransportType.RabbitMq, typeof(OrderRoot).Assembly);
        //builder.AddCustomObservability();
        builder.Services.AddCustomHealthCheck();


        builder.Services.AddEasyCaching(options =>
        {
            options.UseRedis(configuration.GetSection("EasyCaching:Redis"), "redis");
        });

        return builder;
    }

    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        var env = app.Environment;
        var appOptions = app.GetOptions<AppOptions>(nameof(AppOptions));

        //app.UseCustomObservability();

        app.UseCustomProblemDetails();
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = LogEnrichHelper.EnrichFromRequest;
        });
        app.UseCorrelationId();
        //app.UseMigration<OrderDbContext>();
        app.UseCustomHealthCheck();
        app.UseRateLimiter();
        app.MapGet("/", x => x.Response.WriteAsync(appOptions.Name));

        if (env.IsDevelopment())
        {
            app.UseAspnetOpenApi();
        }

        return app;
    }
}
