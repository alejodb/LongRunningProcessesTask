using System;
using LongRunningProcesses.Application.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace LongRunningProcesses.Infrastructure.AsyncCommunications;

public class SignalRCommunicationsProvider(IConfiguration configuration) : IAsyncCommunicationsProvider
{
  public async Task SendResponseMessage(string connectionId, string message)
  {
    await SendMessage("SendResponseMessage", connectionId, message);
  }

  public async Task SendStatusMessage(string connectionId, string message)
  {
    await SendMessage("SendStatusMessage", connectionId, message);
  }

  private async Task SendMessage(string methodName, string connectionId, string message)
  {
    var hubConnection = new HubConnectionBuilder()
        .WithUrl($"{configuration["services:apiservice:http:0"]}/chatHub")
        .Build();
    await hubConnection.StartAsync();
    try
    {
      await hubConnection.InvokeAsync(methodName, connectionId, message);
    }
    finally
    {
      await hubConnection.DisposeAsync();
    }
  }
}
