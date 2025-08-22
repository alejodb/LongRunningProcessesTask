using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Dtos.Queues;
using MassTransit;

namespace LongRunningProcesses.WorkerService;

public class CancelProcessQueueConsumer(
  ILogger<CancelProcessQueueConsumer> logger,
  IMessagesConsumerService messagesConsumerService)
  : IConsumer<CancelProcessMessageDto>
{
  public async Task Consume(ConsumeContext<CancelProcessMessageDto> context)
  {
    logger.LogInformation($"Cancel process message received: {context.Message}");
    await messagesConsumerService.ConsumeCancelProcessMessage(context.Message);
  }
}
