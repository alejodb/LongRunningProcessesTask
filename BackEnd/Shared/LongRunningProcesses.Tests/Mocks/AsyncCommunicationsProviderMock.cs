using LongRunningProcesses.Application.Interfaces;

public class AsyncCommunicationsProviderMock : IAsyncCommunicationsProvider
{
  private readonly List<string> responseMessages = new List<string>();
  private readonly List<string> statusMessages = new List<string>();
  public IReadOnlyList<string> ResponseMessages => responseMessages;
  public IReadOnlyList<string> StatusMessages => statusMessages;

  public Task SendResponseMessage(string connectionId, string message)
  {
    responseMessages.Add(message);
    return Task.CompletedTask;
  }

  public Task SendStatusMessage(string connectionId, string message)
  {
    statusMessages.Add(message);
    return Task.CompletedTask;
  }
}