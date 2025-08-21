using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Domain;
using LongRunningProcesses.Dtos.Queues;
using Microsoft.Extensions.Logging;

namespace LongRunningProcesses.Application.UsesCases;

public class LongRunningProcessesOrchestator(
  ILogger<LongRunningProcessesOrchestator> logger,
  IAsyncCommunicationsProvider asyncCommunicationsProvider,
  TestOcurrencesTasksPlanner testOcurrencesTasksPlanner,
  CancellationMonitor cancellationMonitor,
  IProcessStateRepository processStateRepository)
{
  public async Task StartTextOcurrencesProcess(CountTextOcurrencesMessageDto countTextOcurrencesMessageDto)
  {
    var cancellationTokenSource = new CancellationTokenSource();

    if (!await processStateRepository.CheckIsCanceledAsync(countTextOcurrencesMessageDto.ProcessId))
    {
      var processProgress = await processStateRepository.GetProcessProgressAsync(countTextOcurrencesMessageDto.ProcessId);
      if (processProgress == null)
      {
        processProgress = new ProcessProgress
        {
          ProcessId = countTextOcurrencesMessageDto.ProcessId,
          Position = 0,
          SignedTextOccurrences = testOcurrencesTasksPlanner.GenerateTasksForProcessText(countTextOcurrencesMessageDto.Text),
          ConnectionId = countTextOcurrencesMessageDto.ConnectionId
        };
        await processStateRepository.SaveProcessProgressAsync(processProgress);
      }
      logger.LogInformation($"Starting ProcessId: {processProgress.ProcessId}, Position: {processProgress.Position}");

      Task monitorTask = cancellationMonitor.MonitorCancellationAsync(countTextOcurrencesMessageDto.ProcessId, cancellationTokenSource);
      try
      {
        while (processProgress.Position < processProgress.SignedTextOccurrences.Length)
        {
          await ExecuteLengthyOperation(processProgress, cancellationTokenSource.Token);
        }
        await processStateRepository.RemoveProcessStateAsync(processProgress.ProcessId);
        await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process COMPLETED successfully.");
      }
      catch (OperationCanceledException)
      {
        logger.LogInformation($"Process {processProgress.ProcessId} was canceled.");
        await processStateRepository.RemoveProcessStateAsync(processProgress.ProcessId);
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
        await processStateRepository.RemoveProcessStateAsync(countTextOcurrencesMessageDto.ProcessId);
        await asyncCommunicationsProvider.SendStatusMessage(countTextOcurrencesMessageDto.ConnectionId, $"Long-running process CANCELED.");
    }
  }

  private async Task ExecuteLengthyOperation(ProcessProgress processProgress, CancellationToken cancellationToken)
  {
    var randomizer = new Random();
    await Task.Delay(randomizer.Next(5000), cancellationToken);

    // Uncomment this section for testing random exceptions and retries
    /*if (processProgress.Position == randomizer.Next(0, processProgress.SignedTextOccurrences.Length - 1))
    {
      throw new InvalidOperationException($"Simulated error at position {processProgress.Position} for process {processProgress.ProcessId}");
    }*/

    if (await processStateRepository.CheckIsCanceledAsync(processProgress.ProcessId))
    {
      throw new OperationCanceledException();
    }

    await asyncCommunicationsProvider.SendResponseMessage(processProgress.ConnectionId, processProgress.SignedTextOccurrences[processProgress.Position].ToString());
    logger.LogInformation($"Processed character '{processProgress.SignedTextOccurrences[processProgress.Position]}' at position {processProgress.Position} for process {processProgress.ProcessId}");

    processProgress.Position++;
    await processStateRepository.SaveProcessProgressAsync(processProgress);
  }
}
