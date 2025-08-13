var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var rabbitmq = builder.AddRabbitMQ("rabbitmq");

var apiService = builder.AddProject<Projects.LongRunningProcesses_Api>("apiservice")
.WithReference(redis)
.WithReference(rabbitmq)
.WaitFor(redis)
.WaitFor(rabbitmq);

builder.AddProject<Projects.LongRunningProcesses_WorkerService>("workerservice")
.WithReference(redis)
.WithReference(rabbitmq)
.WithReference(apiService)
.WaitFor(apiService);

builder.AddNpmApp("long-running-processes-client", "../long-running-processes-client")
    .WithReference(apiService);

builder.Build().Run();
