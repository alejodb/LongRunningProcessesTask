using System;

namespace LongRunningProcesses.Application.UsesCases;

public class LengthyOperationTask
{
  public async Task ExecuteAsync(string processId)
  {
    // Simulate a lengthy operation
    await Task.Delay(TimeSpan.FromSeconds(10));
  }
}
