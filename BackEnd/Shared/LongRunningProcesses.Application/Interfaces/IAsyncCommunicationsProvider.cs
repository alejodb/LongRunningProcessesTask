namespace LongRunningProcesses.Application.Interfaces;

public interface IAsyncCommunicationsProvider
{
  Task SendResponseMessage(string connectionId, StepCompletedMessageDto stepCompletedMessageDto);
  Task SendStatusMessage(string connectionId, string message);
  
}
