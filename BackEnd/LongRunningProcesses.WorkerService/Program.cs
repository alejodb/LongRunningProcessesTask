using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Application.Services;
using LongRunningProcesses.Application.UsesCases;
using LongRunningProcesses.Infrastructure.AsyncCommunications;
using LongRunningProcesses.Infrastructure.Persistence;
using LongRunningProcesses.WorkerService;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.AddRedisDistributedCache("redis");

builder.Services.AddMassTransit(x =>
    {
      x.AddConsumer<CountTextOcurrencesQueueConsumer>();
      x.AddConsumer<CancelProcessQueueConsumer>();

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
          e.ConfigureConsumer<CountTextOcurrencesQueueConsumer>(context);
          e.ConfigureConsumer<CancelProcessQueueConsumer>(context);
        });
      });
    });

builder.Services.AddScoped<IProcessStateRepository, RedisProcessStateRepository>();
builder.Services.AddScoped<IAsyncCommunicationsProvider, SignalRCommunicationsProvider>();
builder.Services.AddScoped<IMessagesConsumerService, MessagesConsumerService>();
builder.Services.AddScoped<LongRunningProcessesOrchestator>();
builder.Services.AddScoped<CancellationMonitor>();
builder.Services.AddScoped<TestOcurrencesTasksPlanner>();

var host = builder.Build();
host.Run();
