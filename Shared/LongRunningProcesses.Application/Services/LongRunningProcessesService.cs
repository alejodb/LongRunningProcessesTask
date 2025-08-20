using LongRunningProcesses.Dtos.Apis;
using LongRunningProcesses.Dtos.Queues;
using LongRunningProcesses.Application.Interfaces;
using Microsoft.Extensions.Logging;
using LongRunningProcesses.Domain;
using System.Text;
using System.Text.Json;

namespace LongRunningProcesses.Application.Services;

public class LongRunningProcessesService(ILogger<LongRunningProcessesService> logger, IMessagePublisher messagePublisher, IProcessStateRepository processStateRepository, IAsyncCommunicationsProvider asyncCommunicationsProvider) : ILongRunningProcessesService
{
  public async Task<CountTextOcurrencesResponseDto> CountTextOcurrences(CountTextOcurrencesRequestDto countTextOcurrencesRequestDto)
  {
    var processState = new ProcessState
    {
      ProcessId = Guid.NewGuid().ToString("N"),
      Status = ProcessStatus.Pending,
      ProgressPosition = 0
    };

    logger.LogInformation($"Received request to count text occurrences, ProcessId: {processState.ProcessId}, ConnectionId: {countTextOcurrencesRequestDto.ConnectionId}");
    await processStateRepository.SaveAsync(processState);

    await messagePublisher.Publish(new CountTextOcurrencesMessageDto(processState.ProcessId, countTextOcurrencesRequestDto.Text, countTextOcurrencesRequestDto.ConnectionId));

    return new CountTextOcurrencesResponseDto(processState.ProcessId);
  }

  public async Task CancelProcess(string processId)
  {
    var processState = await processStateRepository.GetOrInitializeAsync(processId);
    if (processState != null)
    {
      processState.Status = ProcessStatus.Canceled;
      await processStateRepository.SaveAsync(processState);
    }
  }

  public async Task ProcessMessage(CountTextOcurrencesMessageDto message)
  {
    try
    {
      await GenerateAndSendSignedTextOcurrences(message);
    }
    catch (Exception ex)
    {
      await processStateRepository.SaveAsync(new ProcessState
      {
        ProcessId = message.ProcessId,
        Status = ProcessStatus.Failed
      });
      logger.LogError(ex, $"Error processing message for process {message.ProcessId}");
      throw;
    }
  }

  private async Task GenerateAndSendSignedTextOcurrences(CountTextOcurrencesMessageDto countTextOcurrencesMessageDto)
  {
    var processState = await processStateRepository.GetOrInitializeAsync(countTextOcurrencesMessageDto.ProcessId);

    if (processState.Status == ProcessStatus.Processing)
    {
      logger.LogInformation($"Resuming process {processState.ProcessId} from character position {processState.ProgressPosition}");
    }
    else
    {
      logger.LogInformation($"Starting process {countTextOcurrencesMessageDto.ProcessId} for text: {countTextOcurrencesMessageDto.Text}");
    }

    var signedTextOcurrences = GenerateSignedTextOcurrencesString(countTextOcurrencesMessageDto.Text);
    logger.LogInformation($"Process: {JsonSerializer.Serialize(processState)}");
    logger.LogInformation($"Generated signed text occurrences for process {countTextOcurrencesMessageDto.ProcessId}: {signedTextOcurrences}");
    while (processState.ProgressPosition < signedTextOcurrences.Length && processState.Status != ProcessStatus.Canceled)
    {
      processState = await ExecuteProcessStep(processState.ProcessId, signedTextOcurrences[processState.ProgressPosition], processState.ProgressPosition, countTextOcurrencesMessageDto);
    }
    if (processState.Status == ProcessStatus.Canceled)
    {
      logger.LogInformation($"Process {processState.ProcessId} was canceled.");
      await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process CANCELED successfully.");
    }
    else
    {
      await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process COMPLETED successfully.");
    }

    await processStateRepository.RemoveAsync(processState.ProcessId);
  }

  private async Task<ProcessState> ExecuteProcessStep(string processId, char character, int stepPosition, CountTextOcurrencesMessageDto countTextOcurrencesMessageDto)
  {
    var randomizer = new Random();
    await Task.Delay(randomizer.Next(5000));

    // Uncomment this line for testing exceptions and retries
    /*if (position == 5)
    {
      throw new InvalidOperationException($"Simulated error at position {position} for process {message.ProcessId}");
    }*/

    var processState = await processStateRepository.GetOrInitializeAsync(processId);
    if (processState.Status != ProcessStatus.Canceled)
    {
      processState.ProgressPosition++;
      await asyncCommunicationsProvider.SendResponseMessage(countTextOcurrencesMessageDto.ConnectionId, character.ToString());
      await processStateRepository.SaveAsync(processState);

      logger.LogInformation($"Processed character '{character}' at position {processState.ProgressPosition} for process {processState.ProcessId}");
    }
    return processState;
  }

  private string GenerateSignedTextOcurrencesString(string text)
  {
    var base64EncodedText = GenerateBase64EncodedText(text);
    var textOcurrences = GenerateTextOcurrencesString(text);
    var signedTextOcurrences = $"{textOcurrences}/{base64EncodedText}";

    return signedTextOcurrences;
  }

  private string GenerateTextOcurrencesString(string text)
  {
    return FormatCharacterCount(CountTextCharactersOcurrences(text));
  }

  private IDictionary<char, int> CountTextCharactersOcurrences(string text)
  {
    var charactersCount = new Dictionary<char, int>();

    foreach (var character in text)
    {
      if (charactersCount.ContainsKey(character))
      {
        charactersCount[character]++;
      }
      else
      {
        charactersCount[character] = 1;
      }
    }

    return charactersCount.OrderBy(keyValuePair => keyValuePair.Key)
                          .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
  }

  private string FormatCharacterCount(IDictionary<char, int> characterCounts)
  {
    return string.Join("", characterCounts.Select(keyValuePair => $"{keyValuePair.Key}{keyValuePair.Value}"));
  }

  private string GenerateBase64EncodedText(string text)
  {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
  }
}