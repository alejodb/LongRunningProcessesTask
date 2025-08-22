using LongRunningProcesses.Domain;

namespace LongRunningProcesses.Application.Interfaces;

public interface IProcessStateRepository
{
  Task<ProcessProgress?> GetProcessProgressAsync(string processId);
  Task SaveProcessProgressAsync(ProcessProgress processState);
  Task RemoveProcessStateAsync(string processId);
  Task<bool> CheckIsCanceledAsync(string processId);
  Task SaveCanceledAsync(string processId);
}
