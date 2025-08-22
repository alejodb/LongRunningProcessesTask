using LongRunningProcesses.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace LongRunningProcesses.Application.UsesCases;

public class CancellationMonitor(
  ILogger<CancellationMonitor> logger,
  IProcessStateRepository processStateRepository)
{
  private const int CheckIntervalMilliseconds = 500;

  public async Task MonitorCancellationAsync(string processId, CancellationTokenSource cancellationTokenSource)
  {
    try
    {
      while (!cancellationTokenSource.Token.IsCancellationRequested)
      {
        await Task.Delay(CheckIntervalMilliseconds);

        if (await processStateRepository.CheckIsCanceledAsync(processId))
        {
          logger.LogInformation($"Process {processId} has been canceled.");
          cancellationTokenSource.Cancel();
          break;
        }
      }
    }
    catch (OperationCanceledException ex)
    {
      logger.LogError(ex, $"Cancellation monitor for process {processId} finishing by cancellation.");
    }
  }
}