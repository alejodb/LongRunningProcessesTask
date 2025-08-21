using Microsoft.Extensions.Logging;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Dtos.Queues;
using LongRunningProcesses.Domain;
using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Application.UsesCases;

namespace LongRunningProcesses.Application.Services;

public class LongRunningProcessesService(
  ILogger<LongRunningProcessesService> logger,
  IMessagePublisher messagePublisher,
  IProcessStateRepository processStateRepository,
  LongRunningProcessesOrchestator longRunningProcessesOrchestator) : ILongRunningProcessesService
{
  public async Task<CountTextOcurrencesResponseDto> CountTextOcurrences(CountTextOcurrencesRequestDto countTextOcurrencesRequestDto)
  {
    var processState = new ProcessState
    {
      ProcessId = Guid.NewGuid().ToString("N"),
      ProgressPosition = 0
    };

    logger.LogInformation($"Received request to count text occurrences, ProcessId: {processState.ProcessId}, ConnectionId: {countTextOcurrencesRequestDto.ConnectionId}");
    await processStateRepository.SaveAsync(processState);

    await messagePublisher.Publish(new CountTextOcurrencesMessageDto(processState.ProcessId, countTextOcurrencesRequestDto.Text, countTextOcurrencesRequestDto.ConnectionId));

    return new CountTextOcurrencesResponseDto(processState.ProcessId);
  }

  public async Task CancelCountTextOcurrencesProcess(string processId)
  {
    // TODO: Next iteration create new message type in RabbitMQ and perform repository changes in the worker
    var processState = await processStateRepository.GetOrInitializeAsync(processId);
    if (processState != null)
    {
      processState.Canceled = true;
      await processStateRepository.SaveAsync(processState);
    }
  }

  public async Task ProcessCountTextOcurrencesMessage(CountTextOcurrencesMessageDto message)
  {
      await longRunningProcessesOrchestator.StartTextOcurrencesProcess(message);
  }


}