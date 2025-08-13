using System;
using MassTransit;
using LongRunningProcesses.Dtos.QueueDtos;

namespace LongRunningProcesses.WorkerService;

public class ProcessQueueConsumer : IConsumer<CountTextOcurrencesMessageDto>
{
  private readonly ILogger<ProcessQueueConsumer> _logger;

  private readonly IServiceProvider _serviceProvider;
  public ProcessQueueConsumer(ILogger<ProcessQueueConsumer> logger, IServiceProvider serviceProvider)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  public async Task Consume(ConsumeContext<CountTextOcurrencesMessageDto> context)
  {
    _logger.LogInformation("New message received for processing: {ProcessId}", context.Message.ProcessId);
    TextOcurrencesProcessor textOcurrencesProcessor = ActivatorUtilities.CreateInstance<TextOcurrencesProcessor>(_serviceProvider);
    await textOcurrencesProcessor.ProcessMessage(context.Message);
  }
}
