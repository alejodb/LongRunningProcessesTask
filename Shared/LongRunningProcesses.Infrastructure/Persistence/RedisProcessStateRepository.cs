using System.Text.Json;
using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Domain;
using Microsoft.Extensions.Caching.Distributed;

public class RedisProcessStateRepository(IDistributedCache redisCache) : IProcessStateRepository
{
  public async Task<ProcessState> GetOrInitializeAsync(string processId)
  {
    var processState = JsonSerializer.Deserialize<ProcessState>((await redisCache.GetStringAsync($"process:{processId}")) ?? "");
    if (processState == null)
    {
      processState = new ProcessState
      {
        ProcessId = processId,
        Status = ProcessStatus.Pending,
        ProgressPosition = 0
      };
      await SaveAsync(processState);
    }
    return processState;
  }

  public async Task SaveAsync(ProcessState processState)
  {
    await redisCache.SetStringAsync($"process:{processState.ProcessId}", JsonSerializer.Serialize(processState));
  }

  public async Task RemoveAsync(string processId)
  {
    await redisCache.RemoveAsync($"process:{processId}");
  }
}