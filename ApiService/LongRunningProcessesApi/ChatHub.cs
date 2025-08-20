using Microsoft.AspNetCore.SignalR;
using System;

namespace LongRunningProcessesApi
{
  public class ChatHub : Hub
  {
    public async Task SendResponseMessage(string connectionId, string message)
    {
      Console.WriteLine($"Sending message to connection {connectionId}: {message}");
      await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
    }

    public async Task SendStatusMessage(string connectionId, string message)
    {
      Console.WriteLine($"Sending status message to connection {connectionId}: {message}");
      await Clients.Client(connectionId).SendAsync("StatusMessage", message);
    }
  }
}