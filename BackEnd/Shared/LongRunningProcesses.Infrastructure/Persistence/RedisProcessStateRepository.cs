using System.Text.Json;
using LongRunningProcesses.Application.Interfaces;
using LongRunningProcesses.Domain;
using Microsoft.Extensions.Caching.Distributed;

namespace LongRunningProcesses.Infrastructure.Persistence;

public class RedisProcessStateRepository(
  IDistributedCache redisCache)
  : IProcessStateRepository
{
  public async Task<ProcessProgress?> GetProcessProgressAsync(string processId)
  {
    var processStateValue = await redisCache.GetStringAsync($"process:{processId}");
    return !string.IsNullOrEmpty(processStateValue) ? JsonSerializer.Deserialize<ProcessProgress>(processStateValue) : null;
  }

  public async Task SaveProcessProgressAsync(ProcessProgress processProgress)
  {
    await redisCache.SetStringAsync($"process:{processProgress.ProcessId}", JsonSerializer.Serialize(processProgress));
  }

  public async Task RemoveProcessStateAsync(string processId)
  {
    await redisCache.RemoveAsync($"process:{processId}");
    await redisCache.RemoveAsync($"process:{processId}:canceled");
  }

  public async Task<bool> CheckIsCanceledAsync(string processId)
  {
    return !string.IsNullOrEmpty(await redisCache.GetStringAsync($"process:{processId}:canceled"));
  }

  public async Task SaveCanceledAsync(string processId)
  {
    await redisCache.SetStringAsync($"process:{processId}:canceled", "true");
  }
}