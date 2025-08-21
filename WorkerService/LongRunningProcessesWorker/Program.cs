using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Application.Services;
using LongRunningProcesses.Application.UsesCases;
using LongRunningProcesses.Infrastructure.AsyncCommunications;
using LongRunningProcesses.Infrastructure.Messaging;
using LongRunningProcesses.Infrastructure.Persistence;
using LongRunningProcessesWorker;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddRedisDistributedCache("redis");

builder.Services.AddMassTransit(x =>
    {
      x.AddConsumer<ProcessQueueConsumer>();

      x.UsingRabbitMq((context, config) =>
      {
        config.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        config.ReceiveEndpoint("process-queue", e =>
        {
          e.UseMessageRetry(r => r.Incremental(
                retryLimit: 5,
                initialInterval: TimeSpan.FromSeconds(5),
                intervalIncrement: TimeSpan.FromSeconds(10)
            ));
          e.ConfigureConsumer<ProcessQueueConsumer>(context);
        });
      });
    });

builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IProcessStateRepository, RedisProcessStateRepository>();
builder.Services.AddScoped<IAsyncCommunicationsProvider, SignalRCommunicationsProvider>();
builder.Services.AddTransient<ILongRunningProcessesService, LongRunningProcessesService>();
builder.Services.AddTransient<LongRunningProcessesOrchestator>();
builder.Services.AddTransient<CancellationMonitor>();
builder.Services.AddTransient<TestOcurrencesTasksPlanner>();

var host = builder.Build();
host.Run();
