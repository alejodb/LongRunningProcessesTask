using System;
using LongRunningProcesses.Domain;

namespace LongRunningProcesses.Application.Interfaces;

public interface IProcessStateRepository
{
  Task<ProcessState> GetOrInitializeAsync(string processId);
  Task SaveAsync(ProcessState processState);
  Task RemoveAsync(string processId);
}
