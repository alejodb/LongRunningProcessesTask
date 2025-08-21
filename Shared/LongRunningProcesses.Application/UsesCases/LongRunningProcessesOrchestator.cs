using System;
using System.Text.Json;
using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Domain;
using LongRunningProcesses.Dtos.Queues;
using Microsoft.Extensions.Logging;

namespace LongRunningProcesses.Application.UsesCases;

public class LongRunningProcessesOrchestator(ILogger<LongRunningProcessesOrchestator> logger, IAsyncCommunicationsProvider asyncCommunicationsProvider, TestOcurrencesTasksPlanner testOcurrencesTasksPlanner, CancellationMonitor cancellationMonitor, IProcessStateRepository processStateRepository)
{
  public async Task StartTextOcurrencesProcess(CountTextOcurrencesMessageDto countTextOcurrencesMessageDto)
  {
    var cancellationTokenSource = new CancellationTokenSource();

    var processState = await processStateRepository.GetOrInitializeAsync(countTextOcurrencesMessageDto.ProcessId);
    logger.LogInformation($"Process: {JsonSerializer.Serialize(processState)}");

    if (!processState.Canceled)
    {
      var signedTextOcurrences = testOcurrencesTasksPlanner.GenerateTasksForProcessText(countTextOcurrencesMessageDto.Text);
      logger.LogInformation($"Generated signed text occurrences for process {countTextOcurrencesMessageDto.ProcessId}: {signedTextOcurrences}");

      Task monitorTask = cancellationMonitor.MonitorCancellationAsync(countTextOcurrencesMessageDto.ProcessId, cancellationTokenSource);
      try
      {
        while (processState.ProgressPosition < signedTextOcurrences.Length)
        {
          processState = await ExecuteProcessStep(processState.ProcessId, signedTextOcurrences[processState.ProgressPosition], processState.ProgressPosition, countTextOcurrencesMessageDto, cancellationTokenSource.Token);
        }
        await processStateRepository.RemoveAsync(processState.ProcessId);
        await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process COMPLETED successfully.");
      }
      catch (OperationCanceledException)
      {
        logger.LogInformation($"Process {processState.ProcessId} was canceled.");
        await processStateRepository.RemoveAsync(processState.ProcessId);
        await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process CANCELED.");
      }
      finally
      {
        if (!cancellationTokenSource.IsCancellationRequested)
        {
          cancellationTokenSource.Cancel();
        }
        await monitorTask;
        cancellationTokenSource.Dispose();
      }
    }
    else // in the case the cancelation was generated when the process was down
    {
        await processStateRepository.RemoveAsync(processState.ProcessId);
        await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process CANCELED.");
    }
  }

  private async Task<ProcessState> ExecuteProcessStep(string processId, char character, int stepPosition, CountTextOcurrencesMessageDto countTextOcurrencesMessageDto, CancellationToken cancellationToken)
  {
    var randomizer = new Random();
    await Task.Delay(randomizer.Next(5000), cancellationToken);

    // Uncomment this line for testing exceptions and retries
    /*if (position == 5)
    {
      throw new InvalidOperationException($"Simulated error at position {position} for process {message.ProcessId}");
    }*/

    var processState = await processStateRepository.GetOrInitializeAsync(processId);
    processState.ProgressPosition++;
    await asyncCommunicationsProvider.SendResponseMessage(countTextOcurrencesMessageDto.ConnectionId, character.ToString());
    await processStateRepository.SaveAsync(processState);
    logger.LogInformation($"Processed character '{character}' at position {processState.ProgressPosition} for process {processState.ProcessId}");
    return processState;
  }
}
