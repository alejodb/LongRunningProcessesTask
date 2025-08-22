using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Domain;

public class ProcessStateRepositoryMock : IProcessStateRepository
{
    private readonly IDictionary<string, object> memory = new Dictionary<string, object>();

  public Task<ProcessProgress?> GetProcessProgressAsync(string processId)
  {
    memory.TryGetValue($"ProcessProgress_{processId}", out var processProgress);
    return Task.FromResult(processProgress as ProcessProgress);
  }

  public Task SaveProcessProgressAsync(ProcessProgress processState)
  {
    memory[$"ProcessProgress_{processState.ProcessId}"] = processState;
    return Task.CompletedTask;
  }

  public Task RemoveProcessStateAsync(string processId)
  {
    memory.Remove($"ProcessProgress_{processId}");
    memory.Remove($"ProcessCanceled_{processId}");
    return Task.CompletedTask;
  }

  public Task<bool> CheckIsCanceledAsync(string processId)
  {
    memory.TryGetValue($"ProcessCanceled_{processId}", out var isCanceled);
    return Task.FromResult(isCanceled as bool? ?? false);
  }

  public Task SaveCanceledAsync(string processId)
  {
    memory[$"ProcessCanceled_{processId}"] = true;
    return Task.CompletedTask;
  }
}