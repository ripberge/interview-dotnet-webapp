using TixTrack.WebApiInterview.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

var db = new ApplicationContext();
ProductRepository.Seed(db);
OrderRepository.Seed(db);

app.MapControllers();
app.Run();
