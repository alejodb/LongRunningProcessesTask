namespace LongRunningProcesses.Application.Interfaces;

public interface IAsyncCommunicationsProvider
{
  Task SendResponseMessage(string connectionId, string message);
  Task SendStatusMessage(string connectionId, string message);
  
}
