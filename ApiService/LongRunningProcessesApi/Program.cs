using LongRunningProcessesApi;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisDistributedCache("redis");

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCORS",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, config) =>
        {
            config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
            config.ConfigureEndpoints(context);
        });
    });

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.UseCors("MyCORS");

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
