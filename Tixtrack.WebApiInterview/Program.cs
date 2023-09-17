using Serilog;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddSingleton<ApplicationContext>()
    .AddSingleton<IProductRepository, InMemoryProductRepository>()
    .AddSingleton<IOrderRepository, InMemoryOrderRepository>();

builder.Services
    .AddScoped<IProductService, ProductServiceImpl>()
    .AddScoped<IOrderService, OrderServiceImpl>()
    .AddScoped<ISalesReportService, SalesReportServiceImpl>();

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
