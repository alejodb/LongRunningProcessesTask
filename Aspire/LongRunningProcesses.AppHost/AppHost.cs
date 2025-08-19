var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var rabbitmq = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin();

var apiService = builder.AddProject<Projects.LongRunningProcessesApi>("apiservice")
.WithReference(redis)
.WithReference(rabbitmq)
.WaitFor(redis)
.WaitFor(rabbitmq);

builder.AddProject<Projects.LongRunningProcessesWorker>("workerservice")
.WithReference(redis)
.WithReference(rabbitmq)
.WithReference(apiService)
.WaitFor(apiService);

builder.AddNpmApp("long-running-processes-client", "../../FrontEnd/long-running-processes-client")
    .WithReference(apiService);

builder.Build().Run();
