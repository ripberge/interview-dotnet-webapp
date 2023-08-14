using Tixtrack.WebApiInterview;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

Database.Seed();

app.MapControllers();
app.Run();
