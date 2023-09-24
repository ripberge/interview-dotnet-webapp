using System.Text.Json.Serialization;
using Serilog;
using TixTrack.WebApiInterview.Repositories;
using TixTrack.WebApiInterview.Repositories.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddDbContext<IApplicationContext, ApplicationContext>();

builder.Services.Scan(scan => 
    scan.FromCallingAssembly()
        .AddClasses(classes => classes.Where(IsRepository))
            .As(type => type.GetInterfaces().Append(typeof(InMemoryRepository)))
            .WithScopedLifetime()
        .AddClasses(classes => classes.Where(IsUseCase))
            .AsSelf()
            .WithScopedLifetime()
        .AddClasses(classes => classes.Where(IsService))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
);

bool IsRepository(Type type) => type.IsSubclassOf(typeof(InMemoryRepository));
bool IsUseCase(Type type) => type.Name.EndsWith("UseCase");
bool IsService(Type type) => type.Name.EndsWith("ServiceImpl");

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
    var repositories = scope.ServiceProvider.GetServices<InMemoryRepository>();
    foreach (var repository in repositories) repository.Seed();
}

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
// TODO: Show health check endpoint in Swagger.
app.MapHealthChecks("/health");

app.Run();