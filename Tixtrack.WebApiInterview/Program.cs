using System.Text.Json.Serialization;
using Serilog;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddDbContext<ApplicationContext>()
    .AddScoped<IProductRepository, InMemoryProductRepository>()
    .AddScoped<IOrderRepository, InMemoryOrderRepository>();

builder.Services
    .AddScoped<IProductService, ProductServiceImpl>()
    .AddScoped<IOrderService, OrderServiceImpl>().AddScoped<CancelOrderUseCase>()
    .AddScoped<ISalesReportService, SalesReportServiceImpl>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationContext>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // TODO: Add services with Scrutor and then seed all repositories of GetServices<InMemoryRepository>().
    (scope.ServiceProvider.GetRequiredService<IProductRepository>() as InMemoryProductRepository)?.Seed();
    (scope.ServiceProvider.GetRequiredService<IOrderRepository>() as InMemoryOrderRepository)?.Seed();
}

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
