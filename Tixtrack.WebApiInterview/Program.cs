using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<ApplicationContext>()
    .AddSingleton<IProductRepository, InMemoryProductRepository>()
    .AddSingleton<IOrderRepository, InMemoryOrderRepository>();

builder.Services
    .AddScoped<IOrderService, OrderServiceImpl>()
    .AddScoped<ISalesReportService, SalesReportServiceImpl>();

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
