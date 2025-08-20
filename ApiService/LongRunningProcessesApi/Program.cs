using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Application.Services;
using LongRunningProcesses.Infrastructure.AsyncCommunications;
using LongRunningProcesses.Infrastructure.Messaging;
using LongRunningProcesses.Infrastructure.Persistence;
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

builder.Services.AddScoped<ILongRunningProcessesService, LongRunningProcessesService>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IProcessStateRepository, RedisProcessStateRepository>();
builder.Services.AddScoped<IAsyncCommunicationsProvider, SignalRCommunicationsProvider>();

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
app.MapHub<SignalRChatHub>("/chatHub");

app.Run();
