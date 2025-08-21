using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Application.Services;
using LongRunningProcesses.Infrastructure.AsyncCommunications;
using LongRunningProcesses.Infrastructure.Messaging;
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

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddScoped<ITextProcessorService, TextProcessorService>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

var app = builder.Build();

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
