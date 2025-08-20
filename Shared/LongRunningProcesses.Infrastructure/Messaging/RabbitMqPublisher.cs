using LongRunningProcesses.Application.Interfaces;
using MassTransit;

namespace LongRunningProcesses.Infrastructure.Messaging;

public class RabbitMqPublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
  public async Task Publish<T>(T message) where T : notnull
  {
    await publishEndpoint.Publish(message);
  }
}