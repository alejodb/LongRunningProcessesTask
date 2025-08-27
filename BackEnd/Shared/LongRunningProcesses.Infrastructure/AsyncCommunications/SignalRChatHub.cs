using Microsoft.AspNetCore.SignalR;

namespace LongRunningProcesses.Infrastructure.AsyncCommunications;

public class SignalRChatHub : Hub
{
  public async Task SendResponseMessage(string connectionId, StepCompletedMessageDto stepCompletedMessageDto)
  {
    Console.WriteLine($"Sending message to connection {connectionId}: {stepCompletedMessageDto}");
    await Clients.Client(connectionId).SendAsync("ReceiveMessage", stepCompletedMessageDto);
  }

  public async Task SendStatusMessage(string connectionId, string message)
  {
    Console.WriteLine($"Sending status message to connection {connectionId}: {message}");
    await Clients.Client(connectionId).SendAsync("StatusMessage", message);
  }
}