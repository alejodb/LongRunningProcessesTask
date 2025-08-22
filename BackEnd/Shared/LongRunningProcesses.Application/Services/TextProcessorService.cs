using Microsoft.Extensions.Logging;
using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Dtos.Queues;
using LongRunningProcesses.Application.Interfaces;

namespace LongRunningProcesses.Application.Services;

public class TextProcessorService(
  ILogger<TextProcessorService> logger,
  IMessagePublisher messagePublisher) : ITextProcessorService
{
  public async Task<CountTextOcurrencesResponseDto> CountTextOcurrences(CountTextOcurrencesRequestDto countTextOcurrencesRequestDto)
  {
    var processId = Guid.NewGuid().ToString("N");
    logger.LogInformation($"Received request to count text occurrences, ProcessId: {processId}, ConnectionId: {countTextOcurrencesRequestDto.ConnectionId}");
    await messagePublisher.Publish(new CountTextOcurrencesMessageDto(processId, countTextOcurrencesRequestDto.Text, countTextOcurrencesRequestDto.ConnectionId));

    return new CountTextOcurrencesResponseDto(processId);
  }

  public async Task CancelCountTextOcurrencesProcess(string processId)
  {
    logger.LogInformation($"Received request to cancel process, ProcessId: {processId}");
    await messagePublisher.Publish(new CancelProcessMessageDto(processId));
  }
}