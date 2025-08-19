using System;
using MassTransit;
using LongRunningProcesses.Dtos.Queues;

namespace LongRunningProcessesWorker;

public class ProcessQueueConsumer(ILogger<ProcessQueueConsumer> logger, IServiceProvider serviceProvider) : IConsumer<CountTextOcurrencesMessageDto>
{
  public async Task Consume(ConsumeContext<CountTextOcurrencesMessageDto> context)
  {
    logger.LogInformation("New message received for processing: {ProcessId}", context.Message.ProcessId);
    TextOcurrencesProcessor textOcurrencesProcessor = ActivatorUtilities.CreateInstance<TextOcurrencesProcessor>(serviceProvider);
    await textOcurrencesProcessor.ProcessMessage(context.Message);
  }
}
