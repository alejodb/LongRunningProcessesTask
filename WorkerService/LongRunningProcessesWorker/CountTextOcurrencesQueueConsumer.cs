using MassTransit;
using LongRunningProcesses.Dtos.Queues;
using LongRunningProcesses.Application.Interfaces;

namespace LongRunningProcessesWorker;

public class CountTextOcurrencesQueueConsumer(
  ILogger<CountTextOcurrencesQueueConsumer> logger,
  IMessagesConsumerService messagesConsumerService)
  : IConsumer<CountTextOcurrencesMessageDto>
{
  public async Task Consume(ConsumeContext<CountTextOcurrencesMessageDto> context)
  {
    logger.LogInformation($"Count text ocurrences message received: {context.Message}");
    await messagesConsumerService.ConsumeCountTextOcurrencesMessage(context.Message);
  }
}
