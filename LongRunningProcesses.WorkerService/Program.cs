using LongRunningProcesses.WorkerService;
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

  builder.Services.AddTransient<TextOcurrencesProcessor>();

var host = builder.Build();
host.Run();
