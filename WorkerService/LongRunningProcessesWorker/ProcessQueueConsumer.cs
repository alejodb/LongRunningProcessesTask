using System;
using MassTransit;
using LongRunningProcesses.Dtos.Queues;
using LongRunningProcesses.Application.Interfaces;

namespace LongRunningProcessesWorker;

public class ProcessQueueConsumer(ILogger<ProcessQueueConsumer> logger, ILongRunningProcessesService longRunningProcessesService) : IConsumer<CountTextOcurrencesMessageDto>
{
  public async Task Consume(ConsumeContext<CountTextOcurrencesMessageDto> context)
  {
    logger.LogInformation("New message received for processing: {ProcessId}", context.Message.ProcessId);
    await longRunningProcessesService.ProcessCountTextOcurrencesMessage(context.Message);
  }
}
